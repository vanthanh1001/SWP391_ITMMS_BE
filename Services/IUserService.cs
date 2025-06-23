using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(RegisterUserDto registerDto);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }
} 