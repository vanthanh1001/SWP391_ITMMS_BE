using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpGet("plans/{id}")]
        public async Task<ActionResult<TreatmentPlan>> GetTreatmentPlan(int id)
        {
            var plan = await _checkoutService.GetTreatmentPlanAsync(id);
            if (plan == null)
            {
                return NotFound(new { message = "Treatment plan not found" });
            }
            return Ok(plan);
        }

        [HttpGet("customer/{customerId}/plans")]
        public async Task<ActionResult<IEnumerable<TreatmentPlan>>> GetCustomerTreatmentPlans(int customerId)
        {
            var plans = await _checkoutService.GetCustomerTreatmentPlansAsync(customerId);
            return Ok(plans);
        }

        [HttpPost("plans")]
        public async Task<ActionResult<TreatmentPlan>> CreateTreatmentPlan(TreatmentPlan plan)
        {
            var createdPlan = await _checkoutService.CreateTreatmentPlanAsync(plan);
            return CreatedAtAction(nameof(GetTreatmentPlan), new { id = createdPlan.Id }, createdPlan);
        }

        [HttpPut("plans/{id}")]
        public async Task<IActionResult> UpdateTreatmentPlan(int id, TreatmentPlan plan)
        {
            if (id != plan.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var success = await _checkoutService.UpdateTreatmentPlanAsync(plan);
            if (!success)
            {
                return NotFound(new { message = "Treatment plan not found" });
            }

            return NoContent();
        }

        [HttpDelete("plans/{id}")]
        public async Task<IActionResult> DeleteTreatmentPlan(int id)
        {
            var success = await _checkoutService.DeleteTreatmentPlanAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Treatment plan not found" });
            }

            return NoContent();
        }

        [HttpPut("plans/{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var success = await _checkoutService.UpdatePaymentStatusAsync(id, request.Status);
            if (!success)
            {
                return NotFound(new { message = "Treatment plan not found" });
            }

            return NoContent();
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
} 