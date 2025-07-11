using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<TreatmentService> Services { get; set; } = new List<TreatmentService>();
    }
} 