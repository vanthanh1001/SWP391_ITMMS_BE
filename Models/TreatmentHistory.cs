using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Models
{
    public class TreatmentHistory
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
} 