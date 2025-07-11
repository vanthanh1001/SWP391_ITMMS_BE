using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_ITMMS_Api.Models
{
    public class TreatmentPlan
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int ServiceId { get; set; }
        public TreatmentService TreatmentService { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 