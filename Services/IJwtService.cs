using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
    }
} 