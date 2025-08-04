using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentServicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public TreatmentServicesController(AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Lấy danh sách tất cả dịch vụ điều trị (Public API cho Guest)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTreatmentServices()
        {
            try
            {
                var services = await _context.TreatmentServices
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.ServiceName)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = services,
                    message = "Lấy danh sách dịch vụ thành công"
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
        /// Lấy thông tin chi tiết dịch vụ theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTreatmentService(int id)
        {
            try
            {
                var service = await _context.TreatmentServices
                    .Where(s => s.Id == id && s.IsActive)
                    .FirstOrDefaultAsync();

                if (service == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy dịch vụ" 
                    });
                }

                return Ok(new { 
                    success = true,
                    data = service,
                    message = "Lấy thông tin dịch vụ thành công"
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
        /// Tạo dịch vụ điều trị mới (Admin/Manager only)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTreatmentService([FromForm] TreatmentServiceDto dto, IFormFile imageFile)
        {
            try
            {
                // Kiểm tra service code đã tồn tại
                var existingService = await _context.TreatmentServices
                    .FirstOrDefaultAsync(s => s.ServiceCode == dto.ServiceCode);
                    
                if (existingService != null)
                {
                    return BadRequest(new { 
                        success = false,
                        message = "Mã dịch vụ đã tồn tại" 
                    });
                }

                string? imageUrl = null;
                if (imageFile != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(imageFile);
                }

                var service = new TreatmentService
                {
                    ServiceName = dto.ServiceName,
                    ServiceCode = dto.ServiceCode,
                    Description = dto.Description,
                    BasePrice = dto.BasePrice,
                    Procedures = dto.Procedures,
                    Requirements = dto.Requirements,
                    DurationDays = dto.DurationDays,
                    SuccessRate = dto.SuccessRate,
                    ImageUrl = imageUrl,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.TreatmentServices.Add(service);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true,
                    data = service,
                    message = "Tạo dịch vụ thành công"
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
        /// Cập nhật dịch vụ điều trị (Admin/Manager only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTreatmentService(int id, [FromForm] TreatmentServiceDto dto, IFormFile imageFile)
        {
            try
            {
                var service = await _context.TreatmentServices.FindAsync(id);
                if (service == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy dịch vụ" 
                    });
                }

                // Kiểm tra service code đã tồn tại (exclude current service)
                var existingService = await _context.TreatmentServices
                    .FirstOrDefaultAsync(s => s.ServiceCode == dto.ServiceCode && s.Id != id);
                    
                if (existingService != null)
                {
                    return BadRequest(new { 
                        success = false,
                        message = "Mã dịch vụ đã tồn tại" 
                    });
                }

                if (imageFile != null)
                {
                    // Nếu có ảnh mới, upload và cập nhật
                    var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile);
                    service.ImageUrl = imageUrl ?? service.ImageUrl;
                }

                service.ServiceName = dto.ServiceName;
                service.ServiceCode = dto.ServiceCode;
                service.Description = dto.Description;
                service.BasePrice = dto.BasePrice;
                service.Procedures = dto.Procedures;
                service.Requirements = dto.Requirements;
                service.DurationDays = dto.DurationDays;
                service.SuccessRate = dto.SuccessRate;
                service.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true,
                    data = service,
                    message = "Cập nhật dịch vụ thành công"
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
        /// Xóa/Ẩn dịch vụ điều trị (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreatmentService(int id)
        {
            try
            {
                var service = await _context.TreatmentServices.FindAsync(id);
                if (service == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy dịch vụ" 
                    });
                }

                // Soft delete - chỉ ẩn dịch vụ
                service.IsActive = false;
                service.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true,
                    message = "Xóa dịch vụ thành công"
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
        /// Tìm kiếm dịch vụ theo từ khóa
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchTreatmentServices([FromQuery] string keyword)
        {
            try
            {
                var services = await _context.TreatmentServices
                    .Where(s => s.IsActive && 
                        (s.ServiceName.Contains(keyword) || 
                         s.Description.Contains(keyword) ||
                         s.ServiceCode.Contains(keyword)))
                    .OrderBy(s => s.ServiceName)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = services,
                    message = $"Tìm thấy {services.Count} dịch vụ"
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
        /// Lấy bảng giá dịch vụ (Public API)
        /// </summary>
        [HttpGet("pricing")]
        public async Task<IActionResult> GetServicePricing()
        {
            try
            {
                var pricing = await _context.TreatmentServices
                    .Where(s => s.IsActive)
                    .Select(s => new {
                        s.Id,
                        s.ServiceName,
                        s.ServiceCode,
                        s.BasePrice,
                        s.DurationDays,
                        s.SuccessRate,
                        Description = s.Description.Length > 100 ? 
                            s.Description.Substring(0, 100) + "..." : s.Description
                    })
                    .OrderBy(s => s.BasePrice)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = pricing,
                    message = "Lấy bảng giá thành công"
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
    }

    // DTO for TreatmentService
    public class TreatmentServiceDto
    {
        public required string ServiceName { get; set; }
        public required string ServiceCode { get; set; }
        public required string Description { get; set; }
        public decimal BasePrice { get; set; }
        public required string Procedures { get; set; }
        public required string Requirements { get; set; }
        public int DurationDays { get; set; }
        public float SuccessRate { get; set; }
        public string? ImageUrl { get; set; } // Link ảnh đại diện dịch vụ, không required
    }
} 