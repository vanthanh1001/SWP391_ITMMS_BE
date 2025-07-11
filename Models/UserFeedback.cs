using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    public class UserFeedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsVisible { get; set; } = true;

        // Navigation properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
    }
} 