using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SWP391_ITMMS_Api.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int MedicalRecordId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Dosage { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Frequency { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Duration { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Instructions { get; set; } = string.Empty;

        // Navigation properties
        [JsonIgnore]
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
    }
} 