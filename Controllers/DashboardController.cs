using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Data;
using System.Linq;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint tổng quan dashboard
        [HttpGet("overview")]
        public IActionResult GetOverview()
        {
            var totalUsers = _context.Users.Count();
            var totalTreatments = _context.TreatmentHistories.Count();
            var totalFeedbacks = _context.UserFeedbacks.Count();
            return Ok(new {
                TotalUsers = totalUsers,
                TotalTreatments = totalTreatments,
                TotalFeedbacks = totalFeedbacks
            });
        }

        // Endpoint báo cáo theo ngày/tháng/năm
        [HttpGet("report")]
        public IActionResult GetReport(string type = "month")
        {
            // Lấy toàn bộ TreatmentHistories ra trước
            var histories = _context.TreatmentHistories.ToList();

            var data = histories
                .GroupBy(t => type == "year" ? t.Date.Year.ToString() : t.Date.ToString("yyyy-MM"))
                .Select(g => new { Period = g.Key, Count = g.Count() })
                .OrderBy(x => x.Period)
                .ToList();

            return Ok(new
            {
                Type = type,
                Data = data
            });
        }
    }
} 