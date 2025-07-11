using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }
            return Ok(appointment);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetDoctorAppointments(int doctorId)
        {
            var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            try
            {
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment);
                return CreatedAtAction(
                    nameof(GetAppointment),
                    new { id = createdAppointment.Id },
                    createdAppointment
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                var success = await _appointmentService.UpdateAppointmentAsync(appointment);
                if (!success)
                {
                    return NotFound(new { message = "Appointment not found" });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var success = await _appointmentService.CancelAppointmentAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Appointment not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var success = await _appointmentService.CompleteAppointmentAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Appointment not found" });
            }
            return NoContent();
        }
    }
} 