using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface IDoctorScheduleService
    {
        Task<List<DoctorTimeSlot>> GenerateTimeSlotsAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<List<DoctorTimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime);
        Task<bool> BookTimeSlotAsync(int doctorId, DateTime date, TimeSpan startTime);
        Task<bool> ReleaseTimeSlotAsync(int doctorId, DateTime date, TimeSpan startTime);
        Task<List<DoctorLeave>> GetDoctorLeavesAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<bool> RequestLeaveAsync(DoctorLeave leave);
        Task<bool> UpdateScheduleAsync(DoctorSchedule schedule);
    }
} 