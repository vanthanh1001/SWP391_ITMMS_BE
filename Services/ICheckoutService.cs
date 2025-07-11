using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public interface ICheckoutService
    {
        Task<TreatmentPlan?> GetTreatmentPlanAsync(int id);
        Task<IEnumerable<TreatmentPlan>> GetCustomerTreatmentPlansAsync(int customerId);
        Task<TreatmentPlan> CreateTreatmentPlanAsync(TreatmentPlan plan);
        Task<bool> UpdateTreatmentPlanAsync(TreatmentPlan plan);
        Task<bool> DeleteTreatmentPlanAsync(int id);
        Task<bool> UpdatePaymentStatusAsync(int planId, string status);
    }
} 