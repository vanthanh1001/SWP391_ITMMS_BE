using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string jwtSecret = "my_very_long_super_secret_key_1234567890!@#$"; // >= 32 ký tự
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Phone) || string.IsNullOrWhiteSpace(dto.Address) ||
                string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return BadRequest(new { error = "All fields are required." });
            }
            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new { error = "Passwords do not match." });
            }
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new { error = "Username already exists" });
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { error = "Email already exists" });

            var user = new User
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Email = dto.Email,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                Role = "user"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                return BadRequest(new { error = "Invalid credentials" });

            // Tạo claims cho token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "user"),
                new Claim("username", user.Username ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "your-app",
                audience: "your-app",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new {
                message = "Login successful",
                role = user.Role,
                token = tokenString,
                user = new {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    fullName = user.FullName,
                    phone = user.Phone,
                    address = user.Address
                }
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null) return BadRequest(new { error = "User not found" });
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
                return BadRequest(new { error = "Old password incorrect" });

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Password changed" });
        }

        // GET: /api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(new {
                user.Id,
                user.Username,
                user.Email,
                user.Address,
                user.Phone,
                dateOfBirth = user.DateOfBirth.ToString("yyyy-MM-dd")
            });
        }

        // PUT: /api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromQuery] int id, [FromBody] UserUpdateDto update)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            if (!string.IsNullOrWhiteSpace(update.Email)) user.Email = update.Email;
            if (!string.IsNullOrWhiteSpace(update.Address)) user.Address = update.Address;
            if (!string.IsNullOrWhiteSpace(update.Phone)) user.Phone = update.Phone;
            if (update.DateOfBirth != default(DateTime)) user.DateOfBirth = update.DateOfBirth;
            if (!string.IsNullOrWhiteSpace(update.Username)) user.Username = update.Username;
            if (!string.IsNullOrWhiteSpace(update.Password)) user.Password = update.Password;
            if (!string.IsNullOrWhiteSpace(update.FullName)) user.FullName = update.FullName;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated" });
        }

        // GET: /api/user/history
        [HttpGet("history")]
        public async Task<IActionResult> GetTreatmentHistory([FromQuery] int userId)
        {
            var history = await _context.TreatmentHistories.Where(h => h.UserId == userId).ToListAsync();
            return Ok(history);
        }

        // POST: /api/user/feedback
        [HttpPost("feedback")]
        public async Task<IActionResult> CreateUserFeedback([FromBody] CreateFeedbackDto dto)
        {
            var feedback = new UserFeedback
            {
                UserId = dto.UserId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Feedback created" });
        }
    }

    public class ChangePasswordModel
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class BlogPostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public interface IBlogRepository
    {
        IEnumerable<BlogPost> GetAll();
        BlogPost GetById(int id);
        void Create(BlogPost post);
        void Update(BlogPost post);
        void Delete(int id);
    }

    public class BlogRepository : IBlogRepository
    {
        private static List<BlogPost> _posts = new List<BlogPost>();

        public IEnumerable<BlogPost> GetAll() => _posts;

        public BlogPost GetById(int id) => _posts.FirstOrDefault(p => p.Id == id);

        public void Create(BlogPost post)
        {
            post.Id = _posts.Count > 0 ? _posts.Max(p => p.Id) + 1 : 1;
            post.CreatedAt = DateTime.Now;
            _posts.Add(post);
        }

        public void Update(BlogPost post)
        {
            var existing = GetById(post.Id);
            if (existing != null)
            {
                existing.Title = post.Title;
                existing.Content = post.Content;
                existing.UpdatedAt = DateTime.Now;
            }
        }

        public void Delete(int id)
        {
            var post = GetById(id);
            if (post != null)
                _posts.Remove(post);
        }
    }

    public interface IBlogService
    {
        IEnumerable<BlogPost> GetAll();
        BlogPost GetById(int id);
        void Create(BlogPostDto dto);
        void Update(int id, BlogPostDto dto);
        void Delete(int id);
    }

    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repo;

        public BlogService(IBlogRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<BlogPost> GetAll() => _repo.GetAll();

        public BlogPost GetById(int id) => _repo.GetById(id);

        public void Create(BlogPostDto dto)
        {
            var post = new BlogPost
            {
                Title = dto.Title,
                Content = dto.Content
            };
            _repo.Create(post);
        }

        public void Update(int id, BlogPostDto dto)
        {
            var post = _repo.GetById(id);
            if (post != null)
            {
                post.Title = dto.Title;
                post.Content = dto.Content;
                post.UpdatedAt = DateTime.Now;
                _repo.Update(post);
            }
        }

        public void Delete(int id) => _repo.Delete(id);
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
} 