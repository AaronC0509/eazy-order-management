using System.Security.Claims;

namespace CustomerPortal.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Admin");
        }
    }
}
