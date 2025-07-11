using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(20)]
        public string? MaritalStatus { get; set; }

        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [StringLength(2000)]
        public string? MedicalHistory { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        public ICollection<Feedback> GivenFeedbacks { get; set; } = new List<Feedback>();
    }
} 