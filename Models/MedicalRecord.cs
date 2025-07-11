using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SWP391_ITMMS_Api.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public int AppointmentId { get; set; }
        
        [StringLength(500)]
        public string? Diagnosis { get; set; }
        
        [StringLength(1000)]
        public string? Symptoms { get; set; }
        
        [StringLength(1000)]
        public string? Treatment { get; set; }
        
        [StringLength(1000)]
        public string? Prescription { get; set; }
        
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public virtual Customer Customer { get; set; } = null!;
        
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; } = null!;
        
        [JsonIgnore]
        public virtual Appointment Appointment { get; set; } = null!;
        
        [JsonIgnore]
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
} 