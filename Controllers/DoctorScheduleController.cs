using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IDoctorScheduleService _scheduleService;
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public DoctorScheduleController(
            IDoctorScheduleService scheduleService,
            AppDbContext context,
            IJwtService jwtService)
        {
            _scheduleService = scheduleService;
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("generate-slots")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GenerateTimeSlots([FromBody] GenerateSlotsDto dto)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _jwtService.GetUserIdFromToken(token);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Token không hợp lệ" });
                var user = await _context.Users.FindAsync(userId.Value);

                if (user == null)
                    return Unauthorized(new { message = "Người dùng không hợp lệ" });

                int doctorId;
                if (user.Role == "Admin")
                {
                    // Admin có thể tạo slot cho bất kỳ doctor nào
                    doctorId = dto.DoctorId ?? 0;
                }
                else if (user.Role == "Doctor")
                {
                    // Doctor chỉ có thể tạo slot cho chính mình
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId.Value);
                    if (doctor == null)
                        return Unauthorized(new { message = "Không tìm thấy thông tin bác sĩ" });
                    doctorId = doctor.Id;
                }
                else
                {
                    return Forbid();
                }

                var timeSlots = await _scheduleService.GenerateTimeSlotsAsync(doctorId, dto.StartDate, dto.EndDate);

                return Ok(new
                {
                    success = true,
                    message = $"Đã tạo {timeSlots.Count} khung giờ cho bác sĩ",
                    data = timeSlots.Select(ts => new
                    {
                        ts.Date,
                        ts.StartTime,
                        ts.EndTime,
                        ts.MaxPatients,
                        ts.CurrentPatients,
                        ts.IsAvailable
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("available-slots/{doctorId}")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime date)
        {
            try
            {
                var timeSlots = await _scheduleService.GetAvailableTimeSlotsAsync(doctorId, date);

                return Ok(new
                {
                    success = true,
                    data = timeSlots.Select(ts => new
                    {
                        startTime = ts.StartTime.ToString(@"hh\:mm"),
                        endTime = ts.EndTime.ToString(@"hh\:mm"),
                        timeSlot = $"{ts.StartTime:hh\\:mm}-{ts.EndTime:hh\\:mm}",
                        availableSlots = ts.MaxPatients - ts.CurrentPatients,
                        isAvailable = ts.IsAvailable && ts.CurrentPatients < ts.MaxPatients
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("request-leave")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> RequestLeave([FromBody] DoctorLeaveDto dto)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _jwtService.GetUserIdFromToken(token);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Token không hợp lệ" });
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId.Value);

                if (doctor == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin bác sĩ" });

                var leave = new DoctorLeave
                {
                    DoctorId = doctor.Id,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    LeaveType = dto.LeaveType,
                    Reason = dto.Reason,
                    IsApproved = false
                };

                await _scheduleService.RequestLeaveAsync(leave);

                return Ok(new
                {
                    success = true,
                    message = "Đã gửi yêu cầu nghỉ phép thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("leaves/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetDoctorLeaves(int doctorId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _jwtService.GetUserIdFromToken(token);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Token không hợp lệ" });
                var user = await _context.Users.FindAsync(userId.Value);

                if (user == null)
                    return Unauthorized(new { message = "Người dùng không hợp lệ" });

                // Kiểm tra quyền
                if (user.Role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId.Value);
                    if (doctor == null || doctor.Id != doctorId)
                        return Forbid();
                }

                var start = startDate ?? DateTime.Today;
                var end = endDate ?? DateTime.Today.AddMonths(1);

                var leaves = await _scheduleService.GetDoctorLeavesAsync(doctorId, start, end);

                return Ok(new
                {
                    success = true,
                    data = leaves.Select(l => new
                    {
                        l.Id,
                        l.StartDate,
                        l.EndDate,
                        l.LeaveType,
                        l.Reason,
                        l.IsApproved
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("approve-leave/{leaveId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveLeave(int leaveId)
        {
            try
            {
                var leave = await _context.DoctorLeaves.FindAsync(leaveId);
                if (leave == null)
                    return NotFound(new { message = "Không tìm thấy yêu cầu nghỉ phép" });

                leave.IsApproved = true;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Đã phê duyệt yêu cầu nghỉ phép"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("schedule/{doctorId}")]
        public async Task<IActionResult> GetDoctorSchedule(int doctorId)
        {
            try
            {
                var schedules = await _context.DoctorSchedules
                    .Where(ds => ds.DoctorId == doctorId && ds.IsActive)
                    .OrderBy(ds => ds.DayOfWeek)
                    .ThenBy(ds => ds.StartTime)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = schedules.Select(s => new
                    {
                        s.Id,
                        dayOfWeek = s.DayOfWeek.ToString(),
                        s.StartTime,
                        s.EndTime,
                        s.MaxPatientsPerSlot,
                        s.SlotDurationMinutes,
                        s.IsActive
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
} 