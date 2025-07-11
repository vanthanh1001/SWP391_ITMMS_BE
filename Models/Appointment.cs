using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "Consultation"; // Consultation, Treatment, Follow-up

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
} 