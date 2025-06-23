using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromQuery] int customerId, [FromBody] CreateAppointmentDto appointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                var appointment = await _appointmentService.CreateAppointmentAsync(customerId, appointmentDto);
                
                return Ok(new 
                { 
                    message = "Đặt lịch hẹn thành công", 
                    appointment = new 
                    {
                        appointment.Id,
                        appointment.AppointmentDate,
                        appointment.TimeSlot,
                        appointment.Type,
                        appointment.Status,
                        appointment.Notes
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                }

                return Ok(new { appointment });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAppointmentsByCustomer(int customerId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByCustomerAsync(customerId);
                return Ok(new { appointments });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
                return Ok(new { appointments });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            try
            {
                var availableSlots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);
                return Ok(new { availableSlots });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusDto statusDto)
        {
            try
            {
                var result = await _appointmentService.UpdateAppointmentStatusAsync(id, statusDto.Status);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                }

                return Ok(new { message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                }

                return Ok(new { message = "Hủy lịch hẹn thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống" });
            }
        }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class RescheduleDto
    {
        public DateTime NewDate { get; set; }
        public string NewTimeSlot { get; set; } = string.Empty;
    }
} 