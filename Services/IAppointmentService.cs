using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id);
    }
} 