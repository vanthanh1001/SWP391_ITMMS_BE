using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public DoctorsController(AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.User.IsActive && d.IsAvailable)
                    .Select(d => new 
                    {
                        d.Id,
                        d.UserId,
                        d.User.FullName,
                        d.User.Email,
                        d.User.Phone,
                        avatarUrl = d.User.AvatarUrl,
                        d.Specialization,
                        d.LicenseNumber,
                        d.Education,
                        d.ExperienceYears,
                        d.Description,
                        d.ConsultationFee,
                        d.IsAvailable
                    })
                    .ToListAsync();

                return Ok(new { doctors });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.Id == id && d.User.IsActive)
                    .Select(d => new 
                    {
                        d.Id,
                        d.UserId,
                        d.User.FullName,
                        d.User.Email,
                        d.User.Phone,
                        d.User.Address,
                        avatarUrl = d.User.AvatarUrl,
                        d.Specialization,
                        d.LicenseNumber,
                        d.Education,
                        d.ExperienceYears,
                        d.Description,
                        d.ConsultationFee,
                        d.IsAvailable
                    })
                    .FirstOrDefaultAsync();

                if (doctor == null)
                {
                    return NotFound(new { message = "Không tìm thấy bác sĩ" });
                }

                return Ok(new { doctor });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("{id}/schedule")]
        public async Task<IActionResult> GetDoctorSchedule(int id, [FromQuery] DateTime? date)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                
                var appointments = await _context.Appointments
                    .Include(a => a.Customer)
                        .ThenInclude(c => c.User)
                    .Where(a => a.DoctorId == id && 
                               a.AppointmentDate.Date == targetDate.Date &&
                               a.Status != "Cancelled")
                    .OrderBy(a => a.TimeSlot)
                    .Select(a => new 
                    {
                        a.Id,
                        a.TimeSlot,
                        a.Type,
                        a.Status,
                        CustomerName = a.Customer.User.FullName,
                        CustomerPhone = a.Customer.User.Phone
                    })
                    .ToListAsync();

                return Ok(new { date = targetDate, appointments });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("{id}/feedback")]
        public async Task<IActionResult> GetDoctorFeedback(int id)
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Customer)
                        .ThenInclude(c => c.User)
                    .Where(f => f.DoctorId == id)
                    .OrderByDescending(f => f.CreatedAt)
                    .Select(f => new 
                    {
                        f.Id,
                        f.Rating,
                        f.Comment,
                        f.CreatedAt,
                        CustomerName = f.Customer.User.FullName
                    })
                    .ToListAsync();

                var averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
                var totalFeedbacks = feedbacks.Count();

                return Ok(new 
                { 
                    averageRating = Math.Round(averageRating, 1),
                    totalFeedbacks,
                    feedbacks 
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("specializations")]
        public async Task<IActionResult> GetSpecializations()
        {
            try
            {
                var specializations = await _context.Doctors
                    .Where(d => d.User.IsActive && d.IsAvailable)
                    .Select(d => d.Specialization)
                    .Distinct()
                    .ToListAsync();

                return Ok(new { specializations });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string? name, [FromQuery] string? specialization)
        {
            try
            {
                var query = _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.User.IsActive && d.IsAvailable);

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(d => d.User.FullName.Contains(name));
                }

                if (!string.IsNullOrEmpty(specialization))
                {
                    query = query.Where(d => d.Specialization.Contains(specialization));
                }

                var doctors = await query
                    .Select(d => new 
                    {
                        d.Id,
                        d.User.FullName,
                        d.Specialization,
                        d.ExperienceYears,
                        d.ConsultationFee,
                        d.Description
                    })
                    .ToListAsync();

                return Ok(new { doctors });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDto)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound(new { message = "Không tìm thấy bác sĩ" });
                }

                doctor.Specialization = updateDto.Specialization ?? doctor.Specialization;
                doctor.Education = updateDto.Education ?? doctor.Education;
                doctor.ExperienceYears = updateDto.ExperienceYears ?? doctor.ExperienceYears;
                doctor.Description = updateDto.Description ?? doctor.Description;
                doctor.ConsultationFee = updateDto.ConsultationFee ?? doctor.ConsultationFee;

                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật thông tin bác sĩ thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPut("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] bool isAvailable)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound(new { message = "Không tìm thấy bác sĩ" });
                }

                doctor.IsAvailable = isAvailable;
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật trạng thái khả dụng thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadDoctorAvatar(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null || doctor.User == null)
                return NotFound(new { message = "Không tìm thấy bác sĩ hoặc user" });
            var imageUrl = await _cloudinaryService.UploadImageAsync(file);
            doctor.User.AvatarUrl = imageUrl;
            _context.Users.Update(doctor.User);
            await _context.SaveChangesAsync();
            return Ok(new { avatarUrl = imageUrl });
        }

        // MANAGEMENT ENDPOINTS - Dành cho Manager để quản lý tất cả doctor
        [HttpGet("management")]
        public async Task<IActionResult> GetAllDoctorsForManagement()
        {
            try
            {
                // TODO: Add authorization check for Manager role
                // if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                // {
                //     return Forbid(new { message = "Chỉ Manager mới có quyền truy cập" });
                // }

                var doctors = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.User.IsActive) // Chỉ lọc User.IsActive, không lọc IsAvailable
                    .Select(d => new 
                    {
                        d.Id,
                        d.UserId,
                        d.User.FullName,
                        d.User.Email,
                        d.User.Phone,
                        d.User.Address,
                        d.User.Role,
                        d.User.CreatedAt,
                        avatarUrl = d.User.AvatarUrl,
                        d.Specialization,
                        d.LicenseNumber,
                        d.Education,
                        d.ExperienceYears,
                        d.Description,
                        d.ConsultationFee,
                        d.IsAvailable,
                        // Thêm thông tin thống kê
                        TotalAppointments = d.Appointments.Count(),
                        CompletedAppointments = d.Appointments.Count(a => a.Status == "Completed"),
                        AverageRating = d.ReceivedFeedbacks.Any() ? d.ReceivedFeedbacks.Average(f => f.Rating) : 0
                    })
                    .OrderBy(d => d.FullName)
                    .ToListAsync();

                return Ok(new { 
                    doctors,
                    totalCount = doctors.Count,
                    availableCount = doctors.Count(d => d.IsAvailable),
                    unavailableCount = doctors.Count(d => !d.IsAvailable)
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("management/{id}")]
        public async Task<IActionResult> GetDoctorForManagement(int id)
        {
            try
            {
                // TODO: Add authorization check for Manager role
                // if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                // {
                //     return Forbid(new { message = "Chỉ Manager mới có quyền truy cập" });
                // }

                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Appointments)
                    .Include(d => d.ReceivedFeedbacks)
                        .ThenInclude(f => f.Customer)
                            .ThenInclude(c => c.User)
                    .Where(d => d.Id == id && d.User.IsActive)
                    .Select(d => new 
                    {
                        d.Id,
                        d.UserId,
                        d.User.FullName,
                        d.User.Email,
                        d.User.Phone,
                        d.User.Address,
                        d.User.Role,
                        d.User.CreatedAt,
                        d.User.UpdatedAt,
                        d.Specialization,
                        d.LicenseNumber,
                        d.Education,
                        d.ExperienceYears,
                        d.Description,
                        d.ConsultationFee,
                        d.IsAvailable,
                        // Thống kê chi tiết
                        TotalAppointments = d.Appointments.Count(),
                        CompletedAppointments = d.Appointments.Count(a => a.Status == "Completed"),
                        CancelledAppointments = d.Appointments.Count(a => a.Status == "Cancelled"),
                        PendingAppointments = d.Appointments.Count(a => a.Status == "Scheduled"),
                        AverageRating = d.ReceivedFeedbacks.Any() ? d.ReceivedFeedbacks.Average(f => f.Rating) : 0,
                        TotalFeedbacks = d.ReceivedFeedbacks.Count(),
                        // Lịch sử feedback gần đây
                        RecentFeedbacks = d.ReceivedFeedbacks
                            .OrderByDescending(f => f.CreatedAt)
                            .Take(5)
                            .Select(f => new 
                            {
                                f.Id,
                                f.Rating,
                                f.Comment,
                                f.CreatedAt,
                                CustomerName = f.Customer.User.FullName
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (doctor == null)
                {
                    return NotFound(new { message = "Không tìm thấy bác sĩ" });
                }

                return Ok(new { doctor });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPut("management/{id}/toggle-availability")]
        public async Task<IActionResult> ToggleDoctorAvailability(int id)
        {
            try
            {
                // TODO: Add authorization check for Manager role
                // if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                // {
                //     return Forbid(new { message = "Chỉ Manager mới có quyền truy cập" });
                // }

                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound(new { message = "Không tìm thấy bác sĩ" });
                }

                doctor.IsAvailable = !doctor.IsAvailable;
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = $"Đã {(doctor.IsAvailable ? "kích hoạt" : "vô hiệu hóa")} bác sĩ thành công",
                    isAvailable = doctor.IsAvailable
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("management/search")]
        public async Task<IActionResult> SearchDoctorsForManagement(
            [FromQuery] string? name, 
            [FromQuery] string? specialization,
            [FromQuery] bool? isAvailable)
        {
            try
            {
                // TODO: Add authorization check for Manager role
                // if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                // {
                //     return Forbid(new { message = "Chỉ Manager mới có quyền truy cập" });
                // }

                var query = _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.User.IsActive); // Chỉ lọc User.IsActive

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(d => d.User.FullName.Contains(name));
                }

                if (!string.IsNullOrEmpty(specialization))
                {
                    query = query.Where(d => d.Specialization.Contains(specialization));
                }

                if (isAvailable.HasValue)
                {
                    query = query.Where(d => d.IsAvailable == isAvailable.Value);
                }

                var doctors = await query
                    .Select(d => new 
                    {
                        d.Id,
                        d.User.FullName,
                        d.User.Email,
                        d.User.Phone,
                        d.Specialization,
                        d.LicenseNumber,
                        d.ExperienceYears,
                        d.ConsultationFee,
                        d.IsAvailable,
                        d.User.CreatedAt,
                        TotalAppointments = d.Appointments.Count(),
                        AverageRating = d.ReceivedFeedbacks.Any() ? d.ReceivedFeedbacks.Average(f => f.Rating) : 0
                    })
                    .OrderBy(d => d.FullName)
                    .ToListAsync();

                return Ok(new { doctors });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }
    }

    public class UpdateDoctorDto
    {
        public string? Specialization { get; set; }
        public string? Education { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Description { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
} 