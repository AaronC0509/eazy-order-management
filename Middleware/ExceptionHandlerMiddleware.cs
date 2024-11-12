using CustomerPortal.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;

namespace CustomerPortal.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    context.Response.ContentType = "application/json";
                    var result = new
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Message = "This action can only be performed by administrators",
                        ErrorType = "ForbiddenAccess"
                    };
                    await context.Response.WriteAsJsonAsync(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException notFound =>
                    (StatusCodes.Status404NotFound, notFound.Message),

                UnauthorizedAccessException unauthorized =>
                    (StatusCodes.Status401Unauthorized, "You are not authorized to perform this action"),

                AuthenticationException auth =>
                    (StatusCodes.Status401Unauthorized, "Authentication failed. Please check your credentials."),

                InvalidOperationException invalid =>
                    (StatusCodes.Status400BadRequest, invalid.Message),

                SecurityTokenExpiredException expired =>
                    (StatusCodes.Status401Unauthorized, "Your session has expired. Please login again"),

                _ => (StatusCodes.Status500InternalServerError, "An error occurred while processing your request")
            };

            context.Response.StatusCode = statusCode;

            var result = new
            {
                StatusCode = statusCode,
                Message = message,
                ErrorType = exception.GetType().Name
            };

            await context.Response.WriteAsJsonAsync(result);
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
