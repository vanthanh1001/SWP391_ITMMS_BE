using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_ITMMS_Api.Models
{
    public class MonthlyReport
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }
        
        public int TotalPatients { get; set; }
        
        public int NewPatients { get; set; }
        
        public int TotalAppointments { get; set; }
        
        public int CompletedAppointments { get; set; }
        
        public int CancelledAppointments { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }
        
        public double AverageRating { get; set; }
        
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
} 