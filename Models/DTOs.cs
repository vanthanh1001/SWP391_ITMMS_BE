using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    // DTOs mới cho Appointment
    public class CancelAppointmentDto
    {
        [Required]
        public string Reason { get; set; }
        
        public string? Notes { get; set; }
        
        [Required]
        public string CancelledBy { get; set; } // "Customer", "Doctor", "System"
    }

    public class RescheduleAppointmentDto
    {
        [Required]
        public DateTime NewDate { get; set; }
        
        [Required]
        public string NewTimeSlot { get; set; }
        
        public string? Reason { get; set; }
        
        [Required]
        public string RequestedBy { get; set; } // "Customer", "Doctor"
    }

    // DTOs mới cho MedicalRecord
    public class VitalSignsDto
    {
        public string? BloodPressure { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Weight { get; set; }
    }

    // DTOs mới cho DoctorSchedule
    public class GenerateSlotsDto
    {
        public int? DoctorId { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
    }

    public class DoctorLeaveDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string LeaveType { get; set; } // "Sick", "Vacation", "Personal"

        [Required]
        public string Reason { get; set; }
    }
} 