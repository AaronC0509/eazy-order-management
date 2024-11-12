using CustomerPortal.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CustomerPortal.Middleware
{
    public class AccountStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public AccountStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
   
            if (context.Request.Path.StartsWithSegments("/api/auth") || context.Request.Path.StartsWithSegments("/api/admin"))
            {
                await _next(context);
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                var isActive = await dbContext.Customers
                    .Where(c => c.Id == userId)
                    .Select(c => c.IsActive)
                    .FirstOrDefaultAsync();

                if (!isActive)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Account is deactivated. You are not allowed to perform any action during deactivation."
                    });
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class AccountStatusMiddlewareExtensions
    {
        public static IApplicationBuilder UseAccountStatusCheck(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AccountStatusMiddleware>();
        }
    }
}
