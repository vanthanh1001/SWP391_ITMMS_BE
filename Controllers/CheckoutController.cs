using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        /// <summary>
        /// Bác sĩ hoàn thành cuộc hẹn và tạo hồ sơ bệnh án
        /// </summary>
        [HttpPost("complete/{doctorId}")]
        public async Task<ActionResult<MedicalRecordResponseDto>> CompleteAppointment(int doctorId, [FromBody] DoctorCompleteAppointmentDto dto)
        {
            try
            {
                var result = await _medicalRecordService.CompleteAppointment(doctorId, dto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new MedicalRecordResponseDto
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Lấy hồ sơ bệnh án của một cuộc hẹn
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecordByAppointment(int appointmentId)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByAppointmentId(appointmentId);
                
                if (record == null)
                {
                    return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án" });
                }
                
                // Format response với tên đầy đủ
                var response = new
                {
                    record.Id,
                    record.CustomerId,
                    record.DoctorId,
                    record.AppointmentId,
                    record.Diagnosis,
                    record.Symptoms,
                    record.Treatment,
                    record.Prescription,
                    record.RecordDate,
                    Doctor = new
                    {
                        record.Doctor.Id,
                        Name = record.Doctor.User.FullName,
                        record.Doctor.Specialization,
                        record.Doctor.LicenseNumber,
                        Phone = record.Doctor.User.Phone,
                        Email = record.Doctor.User.Email
                    },
                    Customer = new
                    {
                        record.Customer.Id,
                        Name = record.Customer.User.FullName,
                        Phone = record.Customer.User.Phone,
                        Email = record.Customer.User.Email,
                        record.Customer.DateOfBirth,
                        record.Customer.Gender
                    }
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy lịch sử khám bệnh của bệnh nhân
        /// </summary>
        [HttpGet("patient/{customerId}/history")]
        public async Task<ActionResult<List<PatientMedicalHistoryDto>>> GetPatientHistory(int customerId)
        {
            try
            {
                var history = await _medicalRecordService.GetPatientMedicalHistory(customerId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy danh sách hồ sơ bệnh án của bác sĩ
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<List<MedicalRecord>>> GetDoctorMedicalRecords(int doctorId)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDoctorId(doctorId);
                
                // Format response với tên bệnh nhân đầy đủ
                var formattedRecords = records.Select(record => new
                {
                    record.Id,
                    record.CustomerId,
                    record.DoctorId,
                    record.AppointmentId,
                    record.Diagnosis,
                    record.Symptoms,
                    record.Treatment,
                    record.Prescription,
                    record.RecordDate,
                    Customer = new
                    {
                        record.Customer.Id,
                        Name = record.Customer.User.FullName,
                        Phone = record.Customer.User.Phone,
                        Email = record.Customer.User.Email,
                        record.Customer.DateOfBirth,
                        record.Customer.Gender
                    },
                    Appointment = new
                    {
                        record.Appointment.Id,
                        record.Appointment.AppointmentDate,
                        record.Appointment.Type,
                        record.Appointment.Status
                    }
                });
                
                return Ok(formattedRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// Test endpoint để kiểm tra trạng thái Medical Records API
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { 
                message = "Medical Records API hoạt động bình thường",
                timestamp = DateTime.Now,
                endpoints = new[]
                {
                    "POST /api/medicalrecords/complete/{doctorId} - Doctor hoàn thành cuộc hẹn",
                    "GET /api/medicalrecords/appointment/{appointmentId} - Xem hồ sơ bệnh án",
                    "GET /api/medicalrecords/patient/{customerId}/history - Lịch sử khám bệnh",
                    "GET /api/medicalrecords/doctor/{doctorId} - Danh sách hồ sơ của bác sĩ"
                }
            });
        }
    }
} 