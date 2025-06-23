using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordResponseDto> CompleteAppointment(int doctorId, DoctorCompleteAppointmentDto dto);
        Task<MedicalRecord?> GetMedicalRecordByAppointmentId(int appointmentId);
        Task<List<PatientMedicalHistoryDto>> GetPatientMedicalHistory(int customerId);
        Task<List<MedicalRecord>> GetMedicalRecordsByDoctorId(int doctorId);
    }
} 