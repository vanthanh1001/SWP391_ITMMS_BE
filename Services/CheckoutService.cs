using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly AppDbContext _context;

        public CheckoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TreatmentPlan?> GetTreatmentPlanAsync(int id)
        {
            return await _context.TreatmentPlans
                .Include(tp => tp.Customer)
                .Include(tp => tp.TreatmentService)
                .FirstOrDefaultAsync(tp => tp.Id == id);
        }

        public async Task<IEnumerable<TreatmentPlan>> GetCustomerTreatmentPlansAsync(int customerId)
        {
            return await _context.TreatmentPlans
                .Include(tp => tp.TreatmentService)
                .Where(tp => tp.CustomerId == customerId)
                .OrderByDescending(tp => tp.CreatedAt)
                .ToListAsync();
        }

        public async Task<TreatmentPlan> CreateTreatmentPlanAsync(TreatmentPlan plan)
        {
            _context.TreatmentPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<bool> UpdateTreatmentPlanAsync(TreatmentPlan plan)
        {
            var existingPlan = await GetTreatmentPlanAsync(plan.Id);
            if (existingPlan == null)
                return false;

            _context.Entry(existingPlan).CurrentValues.SetValues(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTreatmentPlanAsync(int id)
        {
            var plan = await GetTreatmentPlanAsync(id);
            if (plan == null)
                return false;

            _context.TreatmentPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePaymentStatusAsync(int planId, string status)
        {
            var plan = await GetTreatmentPlanAsync(planId);
            if (plan == null)
                return false;

            plan.Status = status;
            plan.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 