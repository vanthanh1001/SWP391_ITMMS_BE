using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentTrackingController : ControllerBase
    {
        private readonly ITreatmentFlowService _treatmentFlowService;

        public TreatmentTrackingController(ITreatmentFlowService treatmentFlowService)
        {
            _treatmentFlowService = treatmentFlowService;
        }

        /// <summary>
        /// 获取患者的完整治疗跟踪信息
        /// </summary>
        [HttpGet("patient/{customerId}/complete-flow")]
        [Authorize]
        public async Task<IActionResult> GetCompleteTreatmentFlow(int customerId)
        {
            try
            {
                var treatmentFlow = await _treatmentFlowService.GetPatientTreatmentFlowAsync(customerId);
                var progressStats = await _treatmentFlowService.GetTreatmentProgressStatsAsync(customerId);
                var timeline = await _treatmentFlowService.GetTreatmentTimelineAsync(customerId);
                var reminders = await _treatmentFlowService.GetTreatmentRemindersAsync(customerId);

                return Ok(new { 
                    success = true, 
                    data = new
                    {
                        TreatmentFlow = treatmentFlow,
                        ProgressStats = progressStats,
                        Timeline = timeline,
                        Reminders = reminders
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取治疗阶段详情
        /// </summary>
        [HttpGet("treatment-plan/{treatmentPlanId}/phase-details")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentPhaseDetails(int treatmentPlanId)
        {
            try
            {
                var nextSteps = await _treatmentFlowService.GetNextTreatmentStepsAsync(treatmentPlanId);
                return Ok(new { success = true, data = nextSteps });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 记录治疗进度更新
        /// </summary>
        [HttpPost("treatment-plan/{treatmentPlanId}/progress-update")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateTreatmentProgress(int treatmentPlanId, [FromBody] SWP391_ITMMS_Api.Services.UpdateTreatmentPhaseDto dto)
        {
            try
            {
                var result = await _treatmentFlowService.UpdateTreatmentPhaseAsync(treatmentPlanId, dto);
                return Ok(new { 
                    success = true, 
                    message = "Cập nhật tiến độ điều trị thành công",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 记录治疗过程中的检查结果
        /// </summary>
        [HttpPost("test-result")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateTestResult([FromBody] SWP391_ITMMS_Api.Services.CreateTestResultDto dto)
        {
            try
            {
                var result = await _treatmentFlowService.CreateTestResultAsync(dto);
                return Ok(new { 
                    success = true, 
                    message = "Tạo kết quả xét nghiệm thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取治疗提醒和通知
        /// </summary>
        [HttpGet("patient/{customerId}/reminders")]
        [Authorize]
        public async Task<IActionResult> GetPatientReminders(int customerId)
        {
            try
            {
                var reminders = await _treatmentFlowService.GetTreatmentRemindersAsync(customerId);
                return Ok(new { success = true, data = reminders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取治疗统计信息
        /// </summary>
        [HttpGet("patient/{customerId}/statistics")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentStatistics(int customerId)
        {
            try
            {
                var stats = await _treatmentFlowService.GetTreatmentProgressStatsAsync(customerId);
                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取治疗时间线
        /// </summary>
        [HttpGet("patient/{customerId}/timeline")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentTimeline(int customerId)
        {
            try
            {
                var timeline = await _treatmentFlowService.GetTreatmentTimelineAsync(customerId);
                return Ok(new { success = true, data = timeline });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
    }
} 