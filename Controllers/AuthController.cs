using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                var user = await _userService.RegisterAsync(registerDto);
                
                return Ok(new 
                { 
                    message = "Đăng ký thành công", 
                    user = new 
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.FullName,
                        user.Phone,
                        user.Role
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
                }

                return Ok(new 
                { 
                    message = "Đăng nhập thành công", 
                    user = new 
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.FullName,
                        user.Phone,
                        user.Role,
                        Doctor = user.Doctor != null ? new 
                        {
                            user.Doctor.Id,
                            user.Doctor.Specialization,
                            user.Doctor.LicenseNumber,
                            user.Doctor.ExperienceYears,
                            user.Doctor.ConsultationFee,
                            user.Doctor.IsAvailable
                        } : null,
                        Customer = user.Customer != null ? new 
                        {
                            user.Customer.Id,
                            user.Customer.DateOfBirth,
                            user.Customer.Gender,
                            user.Customer.MaritalStatus
                        } : null
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmailExists([FromBody] string email)
        {
            try
            {
                var exists = await _userService.EmailExistsAsync(email);
                return Ok(new { exists });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPost("check-username")]
        public async Task<IActionResult> CheckUsernameExists([FromBody] string username)
        {
            try
            {
                var exists = await _userService.UsernameExistsAsync(username);
                return Ok(new { exists });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }
    }
} 