using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;

        public AppointmentsController(IAppointmentService appointmentService, IUserService userService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
        }

        /// <summary>
        /// Đặt lịch hẹn mới (Customer only)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto appointmentDto)
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
                }

                // Chỉ Customer mới được đặt lịch
                var user = CurrentUser;
                if (user?.Role != "Customer" || user.Customer == null)
                {
                    return ForbiddenResponse("Chỉ khách hàng mới được phép đặt lịch hẹn");
                }

                var appointment = await _appointmentService.CreateAppointmentAsync(user.Customer.Id, appointmentDto);
                
                return SuccessResponse(new 
                {
                    appointment = new 
                    {
                        appointment.Id,
                        appointment.AppointmentDate,
                        appointment.TimeSlot,
                        appointment.Type,
                        appointment.Status,
                        appointment.Notes,
                        Doctor = new
                        {
                            appointment.Doctor.Id,
                            appointment.Doctor.User.FullName,
                            appointment.Doctor.Specialization
                        }
                    }
                }, "Đặt lịch hẹn thành công");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy chi tiết lịch hẹn theo ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAppointment(int id)
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                }

                // Kiểm tra quyền truy cập: chỉ customer sở hữu, doctor phụ trách, hoặc admin
                var user = CurrentUser;
                bool hasAccess = false;

                if (user?.Role == "Admin")
                {
                    hasAccess = true;
                }
                else if (user?.Role == "Doctor" && user.Doctor?.Id == appointment.DoctorId)
                {
                    hasAccess = true;
                }
                else if (user?.Role == "Customer" && user.Customer?.Id == appointment.CustomerId)
                {
                    hasAccess = true;
                }

                if (!hasAccess)
                {
                    return ForbiddenResponse("Bạn không có quyền xem lịch hẹn này");
                }

                return SuccessResponse(new { appointment }, "Lấy thông tin lịch hẹn thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn của khách hàng hiện tại (Customer chỉ xem của mình)
        /// </summary>
        [HttpGet("my-appointments")]
        [Authorize]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                var user = CurrentUser;
                
                if (user?.Role == "Customer")
                {
                    if (user.Customer == null)
                    {
                        return BadRequest(new { message = "Không tìm thấy thông tin khách hàng" });
                    }

                    var appointments = await _appointmentService.GetAppointmentsByCustomerAsync(user.Customer.Id);
                    return SuccessResponse(new { appointments }, "Lấy danh sách lịch hẹn thành công");
                }
                else if (user?.Role == "Doctor")
                {
                    if (user.Doctor == null)
                    {
                        return BadRequest(new { message = "Không tìm thấy thông tin bác sĩ" });
                    }

                    var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(user.Doctor.Id);
                    return SuccessResponse(new { appointments }, "Lấy lịch khám của bác sĩ thành công");
                }
                else
                {
                    return ForbiddenResponse("Chỉ khách hàng và bác sĩ mới có thể xem lịch hẹn");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn của khách hàng (Admin/Doctor only)
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentsByCustomer(int customerId)
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                var user = CurrentUser;
                
                // Chỉ admin hoặc doctor mới có thể xem lịch hẹn của khách hàng khác
                if (user?.Role != "Admin" && user?.Role != "Doctor")
                {
                    return ForbiddenResponse("Bạn không có quyền xem thông tin này");
                }

                var appointments = await _appointmentService.GetAppointmentsByCustomerAsync(customerId);
                return SuccessResponse(new { appointments }, "Lấy danh sách lịch hẹn thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn của bác sĩ (Doctor có thể xem lịch của mình, Admin xem tất cả)
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                var user = CurrentUser;
                bool hasAccess = false;

                if (user?.Role == "Admin")
                {
                    hasAccess = true;
                }
                else if (user?.Role == "Doctor" && user.Doctor?.Id == doctorId)
                {
                    // Bác sĩ chỉ được xem lịch khám của chính mình
                    hasAccess = true;
                }

                if (!hasAccess)
                {
                    return ForbiddenResponse("Bạn chỉ có thể xem lịch khám của chính mình");
                }

                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
                return SuccessResponse(new { appointments }, "Lấy danh sách lịch hẹn thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch trình của bác sĩ hiện tại (Doctor only)
        /// </summary>
        [HttpGet("my-schedule")]
        [Authorize]
        public async Task<IActionResult> GetMySchedule()
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                var user = CurrentUser;
                if (user?.Role != "Doctor" || user.Doctor == null)
                {
                    return ForbiddenResponse("Chỉ bác sĩ mới có thể xem lịch trình");
                }

                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(user.Doctor.Id);
                return SuccessResponse(new { appointments }, "Lấy lịch trình thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
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

        /// <summary>
        /// Cập nhật trạng thái lịch hẹn (Doctor/Admin only)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusDto statusDto)
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                // Chỉ doctor hoặc admin mới có thể cập nhật trạng thái
                if (CurrentUser?.Role != "Doctor" && CurrentUser?.Role != "Admin")
                {
                    return ForbiddenResponse("Bạn không có quyền cập nhật trạng thái lịch hẹn");
                }

                var result = await _appointmentService.UpdateAppointmentStatusAsync(id, statusDto.Status);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                }

                return SuccessResponse(null, "Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        /// <summary>
        /// Hủy lịch hẹn (Customer có thể hủy lịch của mình, Doctor/Admin có thể hủy bất kỳ)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                if (!IsAuthenticated)
                {
                    return UnauthorizedResponse();
                }

                // Kiểm tra quyền hủy lịch hẹn
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                }

                var user = CurrentUser;
                bool canCancel = false;

                if (user?.Role == "Admin")
                {
                    canCancel = true;
                }
                else if (user?.Role == "Doctor" && user.Doctor?.Id == appointment.DoctorId)
                {
                    canCancel = true;
                }
                else if (user?.Role == "Customer" && user.Customer?.Id == appointment.CustomerId)
                {
                    canCancel = true;
                }

                if (!canCancel)
                {
                    return ForbiddenResponse("Bạn không có quyền hủy lịch hẹn này");
                }

                var result = await _appointmentService.CancelAppointmentAsync(id);
                if (!result)
                {
                    return BadRequest(new { message = "Không thể hủy lịch hẹn" });
                }

                return SuccessResponse(null, "Hủy lịch hẹn thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
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