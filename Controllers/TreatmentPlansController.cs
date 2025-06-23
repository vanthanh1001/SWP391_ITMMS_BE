using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentPlansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreatmentPlansController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo kế hoạch điều trị mới (Doctor only)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTreatmentPlan([FromBody] CreateTreatmentPlanDto dto)
        {
            try
            {
                // Validate customer and doctor exist
                var customer = await _context.Customers.FindAsync(dto.CustomerId);
                if (customer == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy bệnh nhân" });
                }

                var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
                if (doctor == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy bác sĩ" });
                }

                // Validate treatment service if provided
                TreatmentService? treatmentService = null;
                if (dto.TreatmentServiceId.HasValue)
                {
                    treatmentService = await _context.TreatmentServices.FindAsync(dto.TreatmentServiceId.Value);
                    if (treatmentService == null)
                    {
                        return NotFound(new { success = false, message = "Không tìm thấy dịch vụ điều trị" });
                    }
                }

                var treatmentPlan = new TreatmentPlan
                {
                    CustomerId = dto.CustomerId,
                    DoctorId = dto.DoctorId,
                    TreatmentServiceId = dto.TreatmentServiceId,
                    TreatmentType = dto.TreatmentType,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    TotalCost = treatmentService?.BasePrice ?? dto.TotalCost,
                    Status = "Active",
                    CurrentPhase = 1,
                    PhaseDescription = dto.PhaseDescription ?? "Giai đoạn khởi đầu điều trị",
                    NextPhaseDate = dto.NextPhaseDate,
                    NextVisitDate = dto.NextVisitDate,
                    Notes = dto.Notes ?? "",
                    ProgressNotes = "Bắt đầu kế hoạch điều trị",
                    PaymentStatus = "Pending"
                };

                _context.TreatmentPlans.Add(treatmentPlan);
                await _context.SaveChangesAsync();

                // Return with full details
                var result = await GetTreatmentPlanByIdAsync(treatmentPlan.Id);

                return Ok(new { 
                    success = true,
                    data = result,
                    message = "Tạo kế hoạch điều trị thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Lấy thông tin kế hoạch điều trị theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTreatmentPlan(int id)
        {
            try
            {
                var treatmentPlan = await GetTreatmentPlanByIdAsync(id);
                
                if (treatmentPlan == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy kế hoạch điều trị" 
                    });
                }

                return Ok(new { 
                    success = true,
                    data = treatmentPlan,
                    message = "Lấy thông tin kế hoạch thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Cập nhật tiến trình điều trị (Doctor only)
        /// </summary>
        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdateTreatmentProgress(int id, [FromBody] UpdateProgressDto dto)
        {
            try
            {
                var treatmentPlan = await _context.TreatmentPlans.FindAsync(id);
                if (treatmentPlan == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy kế hoạch điều trị" });
                }

                // Update treatment progress
                treatmentPlan.CurrentPhase = dto.CurrentPhase;
                treatmentPlan.PhaseDescription = dto.PhaseDescription;
                treatmentPlan.NextPhaseDate = dto.NextPhaseDate;
                treatmentPlan.NextVisitDate = dto.NextVisitDate;
                treatmentPlan.Notes = dto.Notes;
                treatmentPlan.ProgressNotes = dto.ProgressNotes;
                
                if (!string.IsNullOrEmpty(dto.Status))
                {
                    treatmentPlan.Status = dto.Status;
                    if (dto.Status == "Completed")
                    {
                        treatmentPlan.EndDate = DateTime.Now;
                    }
                }

                _context.TreatmentPlans.Update(treatmentPlan);
                await _context.SaveChangesAsync();

                var result = await GetTreatmentPlanByIdAsync(id);

                return Ok(new { 
                    success = true,
                    data = result,
                    message = "Cập nhật tiến trình điều trị thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Lấy danh sách kế hoạch điều trị của bệnh nhân
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetTreatmentPlansByCustomer(int customerId)
        {
            try
            {
                var treatmentPlans = await _context.TreatmentPlans
                    .Include(tp => tp.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(tp => tp.TreatmentService)
                    .Where(tp => tp.CustomerId == customerId)
                    .OrderByDescending(tp => tp.StartDate)
                    .Select(tp => new {
                        tp.Id,
                        tp.TreatmentType,
                        tp.Description,
                        tp.StartDate,
                        tp.EndDate,
                        tp.Status,
                        tp.CurrentPhase,
                        tp.PhaseDescription,
                        tp.NextPhaseDate,
                        tp.NextVisitDate,
                        tp.TotalCost,
                        tp.PaidAmount,
                        tp.PaymentStatus,
                        Doctor = new {
                            tp.Doctor.Id,
                            Name = tp.Doctor.User.FullName,
                            tp.Doctor.Specialization
                        },
                        TreatmentService = tp.TreatmentService != null ? new {
                            tp.TreatmentService.Id,
                            tp.TreatmentService.ServiceName,
                            tp.TreatmentService.ServiceCode
                        } : null
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = treatmentPlans,
                    message = $"Tìm thấy {treatmentPlans.Count} kế hoạch điều trị"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Lấy danh sách kế hoạch điều trị của bác sĩ
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetTreatmentPlansByDoctor(int doctorId)
        {
            try
            {
                var treatmentPlans = await _context.TreatmentPlans
                    .Include(tp => tp.Customer)
                        .ThenInclude(c => c.User)
                    .Include(tp => tp.TreatmentService)
                    .Where(tp => tp.DoctorId == doctorId)
                    .OrderByDescending(tp => tp.StartDate)
                    .Select(tp => new {
                        tp.Id,
                        tp.TreatmentType,
                        tp.Description,
                        tp.StartDate,
                        tp.EndDate,
                        tp.Status,
                        tp.CurrentPhase,
                        tp.PhaseDescription,
                        tp.NextPhaseDate,
                        tp.NextVisitDate,
                        tp.TotalCost,
                        tp.PaidAmount,
                        tp.PaymentStatus,
                        tp.Notes,
                        tp.ProgressNotes,
                        Customer = new {
                            tp.Customer.Id,
                            Name = tp.Customer.User.FullName,
                            tp.Customer.User.Phone,
                            tp.Customer.User.Email
                        },
                        TreatmentService = tp.TreatmentService != null ? new {
                            tp.TreatmentService.Id,
                            tp.TreatmentService.ServiceName,
                            tp.TreatmentService.ServiceCode
                        } : null
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = treatmentPlans,
                    message = $"Tìm thấy {treatmentPlans.Count} kế hoạch điều trị"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Lấy kế hoạch điều trị đang active của bệnh nhân
        /// </summary>
        [HttpGet("customer/{customerId}/active")]
        public async Task<IActionResult> GetActiveTreatmentPlan(int customerId)
        {
            try
            {
                var activePlan = await _context.TreatmentPlans
                    .Include(tp => tp.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(tp => tp.TreatmentService)
                    .Where(tp => tp.CustomerId == customerId && tp.Status == "Active")
                    .OrderByDescending(tp => tp.StartDate)
                    .FirstOrDefaultAsync();

                if (activePlan == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không có kế hoạch điều trị đang hoạt động" 
                    });
                }

                var result = new {
                    activePlan.Id,
                    activePlan.TreatmentType,
                    activePlan.Description,
                    activePlan.StartDate,
                    activePlan.Status,
                    activePlan.CurrentPhase,
                    activePlan.PhaseDescription,
                    activePlan.NextPhaseDate,
                    activePlan.NextVisitDate,
                    activePlan.TotalCost,
                    activePlan.PaidAmount,
                    activePlan.PaymentStatus,
                    activePlan.Notes,
                    activePlan.ProgressNotes,
                    Doctor = new {
                        activePlan.Doctor.Id,
                        Name = activePlan.Doctor.User.FullName,
                        activePlan.Doctor.Specialization,
                        activePlan.Doctor.User.Phone
                    },
                    TreatmentService = activePlan.TreatmentService != null ? new {
                        activePlan.TreatmentService.Id,
                        activePlan.TreatmentService.ServiceName,
                        activePlan.TreatmentService.ServiceCode,
                        activePlan.TreatmentService.BasePrice,
                        activePlan.TreatmentService.SuccessRate
                    } : null
                };

                return Ok(new { 
                    success = true,
                    data = result,
                    message = "Lấy kế hoạch điều trị đang hoạt động thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Helper method to get treatment plan with full details
        /// </summary>
        private async Task<object?> GetTreatmentPlanByIdAsync(int id)
        {
            return await _context.TreatmentPlans
                .Include(tp => tp.Customer)
                    .ThenInclude(c => c.User)
                .Include(tp => tp.Doctor)
                    .ThenInclude(d => d.User)
                .Include(tp => tp.TreatmentService)
                .Where(tp => tp.Id == id)
                .Select(tp => new {
                    tp.Id,
                    tp.TreatmentType,
                    tp.Description,
                    tp.StartDate,
                    tp.EndDate,
                    tp.Status,
                    tp.CurrentPhase,
                    tp.PhaseDescription,
                    tp.NextPhaseDate,
                    tp.NextVisitDate,
                    tp.TotalCost,
                    tp.PaidAmount,
                    tp.PaymentStatus,
                    tp.Notes,
                    tp.ProgressNotes,
                    Customer = new {
                        tp.Customer.Id,
                        Name = tp.Customer.User.FullName,
                        tp.Customer.User.Email,
                        tp.Customer.User.Phone,
                        tp.Customer.DateOfBirth,
                        tp.Customer.Gender
                    },
                    Doctor = new {
                        tp.Doctor.Id,
                        Name = tp.Doctor.User.FullName,
                        tp.Doctor.Specialization,
                        tp.Doctor.ExperienceYears,
                        tp.Doctor.User.Phone,
                        tp.Doctor.User.Email
                    },
                    TreatmentService = tp.TreatmentService != null ? new {
                        tp.TreatmentService.Id,
                        tp.TreatmentService.ServiceName,
                        tp.TreatmentService.ServiceCode,
                        tp.TreatmentService.Description,
                        tp.TreatmentService.BasePrice,
                        tp.TreatmentService.DurationDays,
                        tp.TreatmentService.SuccessRate
                    } : null
                })
                .FirstOrDefaultAsync();
        }
    }

    // DTOs for TreatmentPlan
    public class CreateTreatmentPlanDto
    {
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int? TreatmentServiceId { get; set; }
        public string TreatmentType { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public decimal TotalCost { get; set; }
        public string? PhaseDescription { get; set; }
        public DateTime? NextPhaseDate { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateProgressDto
    {
        public int CurrentPhase { get; set; }
        public string PhaseDescription { get; set; } = "";
        public DateTime? NextPhaseDate { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public string Notes { get; set; } = "";
        public string ProgressNotes { get; set; } = "";
        public string? Status { get; set; }
    }
} 