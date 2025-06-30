using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SWP391_ITMMS_Api.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [JsonIgnore]
        public string Password { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Phone]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateTime? DateOfBirth { get; set; }
        
        [Required]
        public string Role { get; set; } = "Customer"; // Guest, Customer, Doctor, Manager, Admin
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Doctor? Doctor { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }

    public class DateOnlyJsonConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            
            var value = reader.GetString();
            return value == null ? null : DateTime.Parse(value);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
        }
    }

    public class Doctor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }
        
        [StringLength(200)]
        public string? Education { get; set; }
        
        public int ExperienceYears { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal ConsultationFee { get; set; }
        
        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        [JsonIgnore]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        [JsonIgnore]
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        [JsonIgnore]
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
        [JsonIgnore]
        public virtual ICollection<Feedback> ReceivedFeedbacks { get; set; } = new List<Feedback>();
    }

    public class Customer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(10)]
        public string? Gender { get; set; } // Male, Female, Other
        
        [StringLength(20)]
        public string? MaritalStatus { get; set; } // Single, Married, Divorced, Widowed
        
        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        
        [StringLength(1000)]
        public string? MedicalHistory { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        [JsonIgnore]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        [JsonIgnore]
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        [JsonIgnore]
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
        [JsonIgnore]
        public virtual ICollection<Feedback> GivenFeedbacks { get; set; } = new List<Feedback>();
    }

    public class TreatmentPlan
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int? TreatmentServiceId { get; set; } // Link to TreatmentService
        
        [Required]
        [StringLength(100)]
        public string TreatmentType { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Completed, Cancelled, On-Hold
        
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalCost { get; set; }
        
        [Column(TypeName = "decimal(12,2)")]
        public decimal PaidAmount { get; set; } = 0;
        
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Partial, Paid, Refunded
        
        // Treatment Monitoring Fields
        public int CurrentPhase { get; set; } = 1; // Phase of treatment (1,2,3...)
        [StringLength(500)]
        public string PhaseDescription { get; set; } = ""; // Current phase description
        public DateTime? NextPhaseDate { get; set; }
        public DateTime? NextVisitDate { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; } = ""; // Doctor's notes and instructions
        
        [StringLength(1000)]
        public string ProgressNotes { get; set; } = ""; // Treatment progress

        // Navigation properties
        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        
        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; }
        
        [ForeignKey("TreatmentServiceId")]
        [JsonIgnore]
        public virtual TreatmentService? TreatmentService { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int? TreatmentPlanId { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        
        [StringLength(20)]
        public string TimeSlot { get; set; } = "";
        
        [StringLength(50)]
        public string Type { get; set; } = "";
        
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, No-Show
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public virtual Customer Customer { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; } = null!;
        
        [ForeignKey("TreatmentPlanId")]
        [JsonIgnore]
        public virtual TreatmentPlan? TreatmentPlan { get; set; }
        
        [JsonIgnore]
        public virtual MedicalRecord? MedicalRecord { get; set; }
        
        [JsonIgnore]
        public virtual Feedback? Feedback { get; set; }
    }

    public class MedicalRecord
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
        
        [StringLength(500)]
        public string? Diagnosis { get; set; }
        
        [StringLength(1000)]
        public string? Symptoms { get; set; }
        
        [StringLength(1000)]
        public string? Treatment { get; set; }
        
        [StringLength(1000)]
        public string? Prescription { get; set; }
        
        public DateTime RecordDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        
        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; }
        
        [ForeignKey("AppointmentId")]
        [JsonIgnore]
        public virtual Appointment Appointment { get; set; }
        
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }

    public class Prescription
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; }
        
        [StringLength(50)]
        public string Dosage { get; set; }
        
        [StringLength(50)]
        public string Frequency { get; set; }
        
        [StringLength(50)]
        public string Duration { get; set; }
        
        [StringLength(500)]
        public string Instructions { get; set; }

        // Navigation properties
        [ForeignKey("MedicalRecordId")]
        public virtual MedicalRecord MedicalRecord { get; set; }
    }

    public class TestResult
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TestType { get; set; } = "";
        
        [Required]
        [StringLength(100)]
        public string TestName { get; set; } = "";
        
        [StringLength(50)]
        public string Results { get; set; } = "";
        
        [StringLength(100)]
        public string? NormalRange { get; set; }
        
        [StringLength(20)]
        public string? Unit { get; set; }
        
        public DateTime TestDate { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        // Navigation properties
        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public virtual Customer Customer { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; } = null!;
    }

    public class Feedback
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int? AppointmentId { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string Comment { get; set; } = "";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public virtual Customer Customer { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; } = null!;
        
        [ForeignKey("AppointmentId")]
        [JsonIgnore]
        public virtual Appointment? Appointment { get; set; }
    }

    // DTO Classes
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = "";
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = "";
        
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = "";
        
        [Required]
        [Phone]
        public string Phone { get; set; } = "";
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        public string Role { get; set; } = "Customer";
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        
        [Required]
        public string Password { get; set; } = "";
    }

    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class CreateAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }
        
        public int? TreatmentPlanId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        public string TimeSlot { get; set; } = "";
        
        [Required]
        public string Type { get; set; } = "";
        
        public string? Notes { get; set; }
    }

    public class CreateFeedbackDto
    {
        [Required]
        public int DoctorId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        public string? Comment { get; set; }
    }

    // Legacy classes for backward compatibility
    public class TreatmentHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = "";
        public DateTime Date { get; set; }
    }

    public class UserFeedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Medical Record Management DTOs
    public class DoctorCompleteAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }
        
        [StringLength(1000)]
        public string Symptoms { get; set; }
        
        [StringLength(500)]
        public string Diagnosis { get; set; }
        
        [StringLength(1000)]
        public string Treatment { get; set; }
        
        [StringLength(1000)]
        public string Prescription { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; }
        
        public bool FollowUpRequired { get; set; }
        
        public DateTime? NextAppointmentDate { get; set; }
    }

    public class MedicalRecordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class PatientMedicalHistoryDto
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string DoctorName { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Prescription { get; set; }
        public string AppointmentType { get; set; }
    }

    // Service Management - Quản lý dịch vụ điều trị
    public class TreatmentService
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; } // IUI, IVF, IVF with ICSI, etc.
        
        [Required]
        [StringLength(50)]
        public string ServiceCode { get; set; } // IUI001, IVF001, etc.
        
        [StringLength(1000)]
        public string Description { get; set; }
        
        [Column(TypeName = "decimal(15,2)")]
        public decimal BasePrice { get; set; }
        
        [StringLength(500)]
        public string Procedures { get; set; } // Quy trình thực hiện
        
        [StringLength(500)]
        public string Requirements { get; set; } // Yêu cầu trước khi thực hiện
        
        public int DurationDays { get; set; } // Thời gian điều trị (ngày)
        public float SuccessRate { get; set; } // Tỷ lệ thành công (%)
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
    }

    // Dashboard Models
    public class DashboardStats
    {
        public int TotalCustomers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedTreatments { get; set; }
        public decimal TotalRevenue { get; set; }
        public float AverageRating { get; set; }
        public int ActiveTreatmentPlans { get; set; }
        public int PendingAppointments { get; set; }
    }

    public class MonthlyReport
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int NewCustomers { get; set; }
        public int CompletedAppointments { get; set; }
        public decimal Revenue { get; set; }
        public int TreatmentPlansStarted { get; set; }
        public int TreatmentPlansCompleted { get; set; }
    }

    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class TreatmentHistoryCreateDto
    {
        public string Description { get; set; }
    }
} 