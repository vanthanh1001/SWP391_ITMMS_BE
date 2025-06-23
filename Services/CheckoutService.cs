using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly AppDbContext _context;

        public MedicalRecordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecordResponseDto> CompleteAppointment(int doctorId, DoctorCompleteAppointmentDto dto)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId && a.DoctorId == doctorId);

            if (appointment == null)
            {
                return new MedicalRecordResponseDto
                {
                    Success = false,
                    Message = "Không tìm thấy cuộc hẹn hoặc bạn không có quyền truy cập"
                };
            }

            if (appointment.Status != "Scheduled")
            {
                return new MedicalRecordResponseDto
                {
                    Success = false,
                    Message = "Cuộc hẹn không ở trạng thái có thể hoàn thành"
                };
            }

            // Kiểm tra xem đã có medical record chưa
            var existingRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(mr => mr.AppointmentId == dto.AppointmentId);

            if (existingRecord != null)
            {
                return new MedicalRecordResponseDto
                {
                    Success = false,
                    Message = "Cuộc hẹn này đã có hồ sơ bệnh án"
                };
            }

            // Tạo medical record
            var medicalRecord = new MedicalRecord
            {
                CustomerId = appointment.CustomerId,
                DoctorId = appointment.DoctorId,
                AppointmentId = appointment.Id,
                Symptoms = dto.Symptoms,
                Diagnosis = dto.Diagnosis,
                Treatment = dto.Treatment,
                Prescription = dto.Prescription,
                RecordDate = DateTime.Now
            };

            _context.MedicalRecords.Add(medicalRecord);

            // Cập nhật appointment status
            appointment.Status = "Completed";
            appointment.CompletedAt = DateTime.Now;
            appointment.Notes = dto.Notes;

            // Nếu cần follow up, ghi chú
            if (dto.FollowUpRequired && dto.NextAppointmentDate.HasValue)
            {
                appointment.Notes += $"\n[Follow-up required: {dto.NextAppointmentDate:dd/MM/yyyy}]";
            }

            await _context.SaveChangesAsync();

            return new MedicalRecordResponseDto
            {
                Success = true,
                Message = "Hoàn thành cuộc hẹn và ghi nhận hồ sơ bệnh án thành công",
                Data = new { 
                    MedicalRecordId = medicalRecord.Id,
                    AppointmentId = appointment.Id,
                    CompletedAt = appointment.CompletedAt,
                    Diagnosis = medicalRecord.Diagnosis,
                    Treatment = medicalRecord.Treatment
                }
            };
        }

        public async Task<MedicalRecord?> GetMedicalRecordByAppointmentId(int appointmentId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Customer)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(mr => mr.AppointmentId == appointmentId);
        }

        public async Task<List<PatientMedicalHistoryDto>> GetPatientMedicalHistory(int customerId)
        {
            var records = await _context.MedicalRecords
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.CustomerId == customerId)
                .OrderByDescending(mr => mr.RecordDate)
                .Select(mr => new PatientMedicalHistoryDto
                {
                    Id = mr.Id,
                    RecordDate = mr.RecordDate,
                    DoctorName = mr.Doctor.User.FullName,
                    Symptoms = mr.Symptoms,
                    Diagnosis = mr.Diagnosis,
                    Treatment = mr.Treatment,
                    Prescription = mr.Prescription,
                    AppointmentType = mr.Appointment.Type
                })
                .ToListAsync();

            return records;
        }

        public async Task<List<MedicalRecord>> GetMedicalRecordsByDoctorId(int doctorId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Customer)
                    .ThenInclude(c => c.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.DoctorId == doctorId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }
    }
} 