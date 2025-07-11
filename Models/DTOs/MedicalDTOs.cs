using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }
        
        public int? TreatmentPlanId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string TimeSlot { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class CreateFeedbackDto
    {
        [Required]
        public int DoctorId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string? Comment { get; set; }
    }

    public class DoctorCompleteAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Symptoms { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Diagnosis { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Treatment { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Prescription { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        public bool FollowUpRequired { get; set; }
        
        public DateTime? NextAppointmentDate { get; set; }
    }

    public class MedicalRecordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

    public class PatientMedicalHistoryDto
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Prescription { get; set; } = string.Empty;
        public string AppointmentType { get; set; } = string.Empty;
    }

    public class TreatmentHistoryCreateDto
    {
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
} 