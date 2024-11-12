using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Response;
using CustomerPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthenticationService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request.Email, request.Password);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
