using System.Security.Claims;

namespace SWP391_ITMMS_Api.Models
{
    public static class UserExtensions
    {
        public static string GetFullName(this User user)
        {
            return $"{user.FirstName} {user.LastName}".Trim();
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.Role);
            return claim?.Value ?? "Guest";
        }
    }
} 