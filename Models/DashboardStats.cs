using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_ITMMS_Api.Models
{
    public class DashboardStats
    {
        [Key]
        public int Id { get; set; }
        
        public int TotalPatients { get; set; }
        
        public int TotalDoctors { get; set; }
        
        public int TotalAppointments { get; set; }
        
        public int CompletedAppointments { get; set; }
        
        public int PendingAppointments { get; set; }
        
        public int CancelledAppointments { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }
        
        public double AverageRating { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
} 