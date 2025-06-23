using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<IEnumerable<UserFeedback>> GetFeedbacks()
        {
            var feedbacks = _context.UserFeedbacks.ToList();
            return Ok(feedbacks);
        }

        // Thêm feedback mới
        [HttpPost]
        public ActionResult<UserFeedback> AddFeedback([FromBody] CreateFeedbackDto dto)
        {
            var feedback = new UserFeedback
            {
                UserId = dto.UserId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserFeedbacks.Add(feedback);
            _context.SaveChanges();
            return Ok(feedback);
        }

        // Xóa feedback
        [HttpDelete("{id}")]
        public IActionResult DeleteFeedback(int id)
        {
            var feedback = _context.UserFeedbacks.Find(id);
            if (feedback == null) return NotFound();
            _context.UserFeedbacks.Remove(feedback);
            _context.SaveChanges();
            return NoContent();
        }
    }
} 