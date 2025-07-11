using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách feedback (Admin/Manager only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFeedbacks()
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Customer).ThenInclude(c => c.User)
                    .Include(f => f.Doctor).ThenInclude(d => d.User)
                    .OrderByDescending(f => f.CreatedAt)
                    .Select(f => new
                    {
                        f.Id,
                        f.Rating,
                        f.Comment,
                        f.CreatedAt,
                        CustomerName = f.Customer.User.FirstName + " " + f.Customer.User.LastName,
                        DoctorName = f.Doctor.User.FirstName + " " + f.Doctor.User.LastName,
                        f.Status
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = feedbacks,
                    message = "Lấy danh sách feedback thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết feedback
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedback(int id)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.Customer).ThenInclude(c => c.User)
                    .Include(f => f.Doctor).ThenInclude(d => d.User)
                    .Where(f => f.Id == id)
                    .Select(f => new
                    {
                        f.Id,
                        f.Rating,
                        f.Comment,
                        f.CreatedAt,
                        CustomerName = f.Customer.User.FirstName + " " + f.Customer.User.LastName,
                        DoctorName = f.Doctor.User.FirstName + " " + f.Doctor.User.LastName,
                        f.Status
                    })
                    .FirstOrDefaultAsync();

                if (feedback == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy feedback"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = feedback,
                    message = "Lấy chi tiết feedback thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Tạo feedback mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDto dto)
        {
            try
            {
                var feedback = new Feedback
                {
                    CustomerId = dto.CustomerId,
                    DoctorId = dto.DoctorId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    data = feedback,
                    message = "Tạo feedback thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Cập nhật feedback
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackDto dto)
        {
            try
            {
                var feedback = await _context.Feedbacks.FindAsync(id);
                if (feedback == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy feedback"
                    });
                }

                feedback.Rating = dto.Rating;
                feedback.Comment = dto.Comment;
                feedback.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    data = feedback,
                    message = "Cập nhật feedback thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Xóa feedback
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                var feedback = await _context.Feedbacks.FindAsync(id);
                if (feedback == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy feedback"
                    });
                }

                feedback.Status = "Deleted";
                feedback.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Xóa feedback thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }
    }

    public class FeedbackDto
    {
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
} 