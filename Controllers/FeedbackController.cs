using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Data;
using System.Collections.Generic;
using System.Linq;
using System;

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

        // Lấy danh sách feedback
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Customer)
                    .ThenInclude(c => c.User)
                .Include(f => f.Doctor)
                    .ThenInclude(d => d.User)
                .Include(f => f.Appointment)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(new { 
                success = true,
                data = feedbacks.Select(f => new {
                    f.Id,
                    f.Rating,
                    f.Comment,
                    f.CreatedAt,
                    CustomerName = f.Customer.User.FullName,
                    DoctorName = f.Doctor.User.FullName,
                    AppointmentDate = f.Appointment != null ? f.Appointment.AppointmentDate.ToString("dd/MM/yyyy") : null
                }),
                message = "Lấy danh sách đánh giá thành công"
            });
        }

        // Thêm feedback mới
        [HttpPost]
        public async Task<ActionResult<Feedback>> AddFeedback([FromBody] CreateFeedbackDto dto)
        {
            var feedback = new Feedback
            {
                CustomerId = dto.DoctorId, // Assuming this is actually the customer ID
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,
                Rating = dto.Rating,
                Comment = dto.Comment ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new {
                success = true,
                data = feedback,
                message = "Thêm đánh giá thành công"
            });
        }

        // Xóa feedback
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound(new {
                    success = false,
                    message = "Không tìm thấy đánh giá"
                });
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return Ok(new {
                success = true,
                message = "Xóa đánh giá thành công"
            });
        }
    }
} 