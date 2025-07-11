using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
    }
} 