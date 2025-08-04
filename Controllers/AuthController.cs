using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;
using SWP391_ITMMS_Api.Data;
using Microsoft.EntityFrameworkCore;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, AppDbContext context, IJwtService jwtService)
        {
            _userService = userService;
            _context = context;
            _jwtService = jwtService;
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

                // Tạo JWT token
                var token = _jwtService.GenerateToken(user);

                return Ok(new 
                { 
                    message = "Đăng nhập thành công", 
                    token = token,
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
                            user.Customer.Address,
                            user.Customer.EmergencyContact
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

        // Các endpoints từ UserController
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
                if (user == null)
                {
                    return NotFound(new { message = "Người dùng không tồn tại" });
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
                {
                    return BadRequest(new { message = "Mật khẩu cũ không đúng" });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Người dùng không tồn tại" });
                }

                return Ok(new 
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.Phone,
                    user.Address,
                    user.Role,
                    user.CreatedAt
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromQuery] int id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                var success = await _userService.UpdateUserAsync(id, updateDto);
                if (!success)
                {
                    return NotFound(new { message = "Người dùng không tồn tại" });
                }

                return Ok(new { message = "Cập nhật profile thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTreatmentHistory([FromQuery] int userId)
        {
            try
            {
                // Get customer treatment plans (history)
                var customerId = await _context.Customers
                    .Where(c => c.UserId == userId)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                if (customerId == 0)
                {
                    return Ok(new { message = "Không tìm thấy lịch sử điều trị", history = new List<object>() });
                }

                var history = await _context.TreatmentPlans
                    .Where(tp => tp.CustomerId == customerId)
                    .Include(tp => tp.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(tp => tp.TreatmentService)
                    .OrderByDescending(tp => tp.StartDate)
                    .Select(tp => new
                    {
                        tp.Id,
                        tp.TreatmentType,
                        tp.Description,
                        tp.StartDate,
                        tp.EndDate,
                        tp.Status,
                        tp.CurrentPhase,
                        tp.TotalCost,
                        tp.PaidAmount,
                        Doctor = new
                        {
                            tp.Doctor.Id,
                            Name = tp.Doctor.User.FullName,
                            tp.Doctor.Specialization
                        },
                        Service = tp.TreatmentService != null ? new
                        {
                            tp.TreatmentService.ServiceName,
                            tp.TreatmentService.ServiceCode
                        } : null
                    })
                    .ToListAsync();

                return Ok(new { history });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPost("feedback")]
        public async Task<IActionResult> CreateUserFeedback([FromBody] CreateFeedbackDto feedbackDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                // Get CustomerId from userId context or appointment
                var customerId = await _context.Customers
                    .Where(c => c.UserId == feedbackDto.UserId)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                if (customerId == 0)
                {
                    return BadRequest(new { message = "Không tìm thấy thông tin customer" });
                }

                var feedback = new Feedback
                {
                    CustomerId = customerId,
                    DoctorId = feedbackDto.DoctorId,
                    AppointmentId = feedbackDto.AppointmentId,
                    Rating = feedbackDto.Rating,
                    Comment = feedbackDto.Comment ?? "",
                    CreatedAt = DateTime.Now
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Gửi feedback thành công", feedbackId = feedback.Id });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }
    }

    // DTOs cho auth controller
    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class CreateFeedbackDto
    {
        public int UserId { get; set; } // User gửi feedback
        public int DoctorId { get; set; }
        public int? AppointmentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
} 