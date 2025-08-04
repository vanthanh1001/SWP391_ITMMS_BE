using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SWP391_ITMMS_Api.Services;
using SWP391_ITMMS_Api.Models;
using System.ComponentModel.DataAnnotations;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentFlowController : ControllerBase
    {
        private readonly ITreatmentFlowService _treatmentFlowService;

        public TreatmentFlowController(ITreatmentFlowService treatmentFlowService)
        {
            _treatmentFlowService = treatmentFlowService;
        }

        /// <summary>
        /// 获取患者的完整诊疗流程
        /// </summary>
        [HttpGet("patient/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetPatientTreatmentFlow(int customerId)
        {
            try
            {
                var treatmentFlow = await _treatmentFlowService.GetPatientTreatmentFlowAsync(customerId);
                return Ok(new { success = true, data = treatmentFlow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        /// <summary>
        /// 更新治疗阶段进度
        /// </summary>
        [HttpPut("treatment-plan/{id}/phase")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateTreatmentPhase(int id, [FromBody] UpdateTreatmentPhaseDto dto)
        {
            try
            {
                var result = await _treatmentFlowService.UpdateTreatmentPhaseAsync(id, dto);
                return Ok(new { 
                    success = true, 
                    message = "Cập nhật giai đoạn điều trị thành công",
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
        /// 记录治疗过程中的医疗记录
        /// </summary>
        [HttpPost("medical-record")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordDto dto)
        {
            try
            {
                var result = await _treatmentFlowService.CreateMedicalRecordAsync(dto);
                return Ok(new { 
                    success = true, 
                    message = "Tạo hồ sơ y tế thành công",
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
        /// 记录检查结果
        /// </summary>
        [HttpPost("test-result")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateTestResult([FromBody] CreateTestResultDto dto)
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
        /// 获取治疗提醒和日程安排
        /// </summary>
        [HttpGet("reminders/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentReminders(int customerId)
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
        /// 获取治疗时间线
        /// </summary>
        [HttpGet("timeline/{customerId}")]
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

        /// <summary>
        /// 获取下一步治疗步骤
        /// </summary>
        [HttpGet("next-steps/{treatmentPlanId}")]
        [Authorize]
        public async Task<IActionResult> GetNextTreatmentSteps(int treatmentPlanId)
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
        /// 获取治疗进度统计
        /// </summary>
        [HttpGet("progress-stats/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetTreatmentProgressStats(int customerId)
        {
            try
            {
                var stats = await _treatmentFlowService.GetTreatmentProgressStatsAsync(customerId);
                return Ok(new { 
                    success = true, 
                    data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
    }

} 