using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_ITMMS_Api.Models
{
    public class TreatmentService
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Duration { get; set; } // Duration in minutes

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
    }
} 