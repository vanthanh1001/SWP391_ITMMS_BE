using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface ITreatmentFlowService
    {
        Task<object> GetPatientTreatmentFlowAsync(int customerId);
        Task<object> UpdateTreatmentPhaseAsync(int treatmentPlanId, UpdateTreatmentPhaseDto dto);
        Task<object> CreateMedicalRecordAsync(CreateMedicalRecordDto dto);
        Task<object> CreateTestResultAsync(CreateTestResultDto dto);
        Task<object> GetTreatmentRemindersAsync(int customerId);
        Task<object> GetTreatmentProgressStatsAsync(int customerId);
        Task<object> GetTreatmentTimelineAsync(int customerId);
        Task<object> GetNextTreatmentStepsAsync(int treatmentPlanId);
    }

    public class UpdateTreatmentPhaseDto
    {
        public int CurrentPhase { get; set; }
        public string PhaseDescription { get; set; } = "";
        public DateTime? NextPhaseDate { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public string ProgressNotes { get; set; } = "";
        public string Notes { get; set; } = "";
        public string? Status { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
        public string Symptoms { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string Treatment { get; set; } = "";
        public string Prescription { get; set; } = "";
        public string Notes { get; set; } = "";
    }

    public class CreateTestResultDto
    {
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public string TestName { get; set; } = "";
        public string TestType { get; set; } = "";
        public string Results { get; set; } = "";
        public string NormalRange { get; set; } = "";
        public string Status { get; set; } = "Normal";
    }
} 