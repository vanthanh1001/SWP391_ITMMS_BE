using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_ITMMS_Api.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Education { get; set; }

        public int ExperienceYears { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; }

        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Feedback> ReceivedFeedbacks { get; set; } = new List<Feedback>();
    }
} 