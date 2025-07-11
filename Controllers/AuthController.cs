using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (await _userService.IsEmailUniqueAsync(request.Email))
            {
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role ?? "Customer",
                    Phone = request.Phone,
                    Address = request.Address,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdUser = await _userService.CreateUserAsync(user);
                return Ok(new { message = "User registered successfully", userId = createdUser.Id });
            }

            return BadRequest(new { message = "Email already exists" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!user.IsActive)
            {
                return BadRequest(new { message = "Account is deactivated" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token, user.Role });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT key not configured");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
} 