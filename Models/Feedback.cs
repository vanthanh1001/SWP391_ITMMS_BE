using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SWP391_ITMMS_Api.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string? Comment { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Customer Customer { get; set; } = null!;
        
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; } = null!;
        
        [JsonIgnore]
        public virtual Appointment? Appointment { get; set; }
    }
} 