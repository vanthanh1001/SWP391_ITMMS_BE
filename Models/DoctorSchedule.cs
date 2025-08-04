using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatientsPerSlot { get; set; } = 1;
        public int SlotDurationMinutes { get; set; } = 60;
        public bool IsActive { get; set; } = true;
        
        public virtual Doctor Doctor { get; set; }
    }

    public class DoctorLeave
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } // "Sick", "Vacation", "Personal"
        public string Reason { get; set; }
        public bool IsApproved { get; set; } = false;
        
        public virtual Doctor Doctor { get; set; }
    }

    public class DoctorTimeSlot
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatients { get; set; }
        public int CurrentPatients { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        public virtual Doctor Doctor { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}

// Complex type cho VitalSigns
namespace SWP391_ITMMS_Api.Models
{
    public class VitalSigns
    {
        public string? BloodPressure { get; set; }  // VD: "120/80"
        public int? HeartRate { get; set; }         // Nhịp tim
        public decimal? Temperature { get; set; }    // Nhiệt độ
        public decimal? Weight { get; set; }         // Cân nặng
    }
} 