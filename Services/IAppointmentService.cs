using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(int customerId, CreateAppointmentDto appointmentDto);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByCustomerAsync(int customerId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<bool> UpdateAppointmentStatusAsync(int id, string status);
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> RescheduleAppointmentAsync(int id, DateTime newDate, string newTimeSlot);
        Task<IEnumerable<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, string timeSlot);
    }
} 