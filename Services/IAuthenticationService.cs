using CustomerPortal.Models.Response;

namespace CustomerPortal.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> LoginAsync(string email, string password);
    }
}
