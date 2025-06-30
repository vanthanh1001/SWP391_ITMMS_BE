using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;
using BCrypt.Net;

namespace SWP391_ITMMS_Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Doctor)
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return user;
        }

        public async Task<User> RegisterAsync(RegisterUserDto registerDto)
        {
            // Check if email or username already exists
            if (await EmailExistsAsync(registerDto.Email))
                throw new InvalidOperationException("Email đã được sử dụng");

            if (await UsernameExistsAsync(registerDto.Username))
                throw new InvalidOperationException("Username đã được sử dụng");

            var user = new User
            {
                Username = registerDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                Phone = registerDto.Phone,
                Address = registerDto.Address ?? "",
                Role = registerDto.Role,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create profile based on role
            if (registerDto.Role == "Customer")
            {
                var customer = new Customer
                {
                    UserId = user.Id,
                    Gender = "",
                    MaritalStatus = "",
                    EmergencyContact = "",
                    MedicalHistory = ""
                };
                _context.Customers.Add(customer);
            }
            else if (registerDto.Role == "Doctor")
            {
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialization = "",
                    LicenseNumber = "",
                    Education = "",
                    ExperienceYears = 0,
                    Description = "",
                    ConsultationFee = 0,
                    IsAvailable = false // Admin needs to approve
                };
                _context.Doctors.Add(doctor);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Doctor)
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Doctor)
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            user.FullName = updateDto.FullName ?? user.FullName;
            user.Phone = updateDto.Phone ?? user.Phone;
            user.Address = updateDto.Address ?? user.Address;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Doctor)
                .Include(u => u.Customer)
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
} 