using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
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