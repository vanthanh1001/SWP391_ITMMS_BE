using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

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

        /// <summary>
        /// Lấy thống kê tổng quan (Manager/Admin only)
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new DashboardStats
                {
                    TotalCustomers = await _context.Customers.CountAsync(),
                    TotalDoctors = await _context.Doctors.Where(d => d.IsAvailable).CountAsync(),
                    TotalAppointments = await _context.Appointments.CountAsync(),
                    CompletedTreatments = await _context.TreatmentPlans.Where(tp => tp.Status == "Completed").CountAsync(),
                    TotalRevenue = await _context.TreatmentPlans.Where(tp => tp.PaymentStatus == "Paid").SumAsync(tp => tp.PaidAmount),
                    AverageRating = await _context.Feedbacks.AverageAsync(f => (float?)f.Rating) ?? 0,
                    ActiveTreatmentPlans = await _context.TreatmentPlans.Where(tp => tp.Status == "Active").CountAsync(),
                    PendingAppointments = await _context.Appointments.Where(a => a.Status == "Scheduled").CountAsync()
                };

                return Ok(new { 
                    success = true,
                    data = stats,
                    message = "Lấy thống kê thành công"
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
        /// Lấy báo cáo theo tháng
        /// </summary>
        [HttpGet("monthly-report")]
        public async Task<IActionResult> GetMonthlyReport([FromQuery] int year = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;

                var monthlyData = new List<MonthlyReport>();

                for (int month = 1; month <= 12; month++)
                {
                    var startDate = new DateTime(year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var report = new MonthlyReport
                    {
                        Month = month,
                        Year = year,
                        NewCustomers = await _context.Customers
                            .Include(c => c.User)
                            .Where(c => c.User.CreatedAt >= startDate && c.User.CreatedAt <= endDate)
                            .CountAsync(),
                        CompletedAppointments = await _context.Appointments
                            .Where(a => a.Status == "Completed" && 
                                       a.CompletedAt >= startDate && a.CompletedAt <= endDate)
                            .CountAsync(),
                        Revenue = await _context.TreatmentPlans
                            .Where(tp => tp.PaymentStatus == "Paid" && 
                                        tp.StartDate >= startDate && tp.StartDate <= endDate)
                            .SumAsync(tp => tp.PaidAmount),
                        TreatmentPlansStarted = await _context.TreatmentPlans
                            .Where(tp => tp.StartDate >= startDate && tp.StartDate <= endDate)
                            .CountAsync(),
                        TreatmentPlansCompleted = await _context.TreatmentPlans
                            .Where(tp => tp.Status == "Completed" && 
                                        tp.EndDate >= startDate && tp.EndDate <= endDate)
                            .CountAsync()
                    };

                    monthlyData.Add(report);
                }

                return Ok(new { 
                    success = true,
                    data = monthlyData,
                    message = $"Lấy báo cáo năm {year} thành công"
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
        /// Lấy thống kê theo dịch vụ
        /// </summary>
        [HttpGet("service-stats")]
        public async Task<IActionResult> GetServiceStats()
        {
            try
            {
                var serviceStats = await _context.TreatmentServices
                    .Where(s => s.IsActive)
                    .Select(s => new {
                        s.Id,
                        s.ServiceName,
                        s.ServiceCode,
                        s.BasePrice,
                        s.SuccessRate,
                        TotalPlans = s.TreatmentPlans.Count(),
                        CompletedPlans = s.TreatmentPlans.Where(tp => tp.Status == "Completed").Count(),
                        ActivePlans = s.TreatmentPlans.Where(tp => tp.Status == "Active").Count(),
                        TotalRevenue = s.TreatmentPlans.Where(tp => tp.PaymentStatus == "Paid").Sum(tp => tp.PaidAmount)
                    })
                    .OrderByDescending(s => s.TotalPlans)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = serviceStats,
                    message = "Lấy thống kê dịch vụ thành công"
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
        /// Lấy thống kê theo bác sĩ
        /// </summary>
        [HttpGet("doctor-stats")]
        public async Task<IActionResult> GetDoctorStats()
        {
            try
            {
                var doctorStats = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.IsAvailable)
                    .Select(d => new {
                        d.Id,
                        DoctorName = d.User.FullName,
                        d.Specialization,
                        d.ExperienceYears,
                        TotalAppointments = d.Appointments.Count(),
                        CompletedAppointments = d.Appointments.Where(a => a.Status == "Completed").Count(),
                        TotalPatients = d.Appointments.Select(a => a.CustomerId).Distinct().Count(),
                        AverageRating = d.ReceivedFeedbacks.Any() ? d.ReceivedFeedbacks.Average(f => f.Rating) : 0,
                        TotalFeedbacks = d.ReceivedFeedbacks.Count(),
                        TotalTreatmentPlans = d.TreatmentPlans.Count(),
                        Revenue = d.TreatmentPlans.Where(tp => tp.PaymentStatus == "Paid").Sum(tp => tp.PaidAmount)
                    })
                    .OrderByDescending(d => d.TotalAppointments)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = doctorStats,
                    message = "Lấy thống kê bác sĩ thành công"
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
        /// Lấy danh sách cuộc hẹn gần đây
        /// </summary>
        [HttpGet("recent-appointments")]
        public async Task<IActionResult> GetRecentAppointments([FromQuery] int limit = 10)
        {
            try
            {
                var recentAppointments = await _context.Appointments
                    .Include(a => a.Customer).ThenInclude(c => c.User)
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(limit)
                    .Select(a => new {
                        a.Id,
                        a.AppointmentDate,
                        a.TimeSlot,
                        a.Type,
                        a.Status,
                        CustomerName = a.Customer.User.FullName,
                        DoctorName = a.Doctor.User.FullName,
                        a.Notes
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = recentAppointments,
                    message = "Lấy danh sách cuộc hẹn gần đây thành công"
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
        /// Lấy thống kê feedback
        /// </summary>
        [HttpGet("feedback-stats")]
        public async Task<IActionResult> GetFeedbackStats()
        {
            try
            {
                var totalFeedbacks = await _context.Feedbacks.CountAsync();
                var averageRating = await _context.Feedbacks.AverageAsync(f => (float?)f.Rating) ?? 0;
                
                var ratingDistribution = await _context.Feedbacks
                    .GroupBy(f => f.Rating)
                    .Select(g => new {
                        Rating = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / totalFeedbacks * 100, 1)
                    })
                    .OrderByDescending(r => r.Rating)
                    .ToListAsync();

                var recentFeedbacks = await _context.Feedbacks
                    .Include(f => f.Customer).ThenInclude(c => c.User)
                    .Include(f => f.Doctor).ThenInclude(d => d.User)
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(5)
                    .Select(f => new {
                        f.Id,
                        f.Rating,
                        f.Comment,
                        f.CreatedAt,
                        CustomerName = f.Customer.User.FullName,
                        DoctorName = f.Doctor.User.FullName
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = new {
                        totalFeedbacks,
                        averageRating = Math.Round(averageRating, 1),
                        ratingDistribution,
                        recentFeedbacks
                    },
                    message = "Lấy thống kê feedback thành công"
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
        /// Lấy thống kê hệ thống theo thời gian thực
        /// </summary>
        [HttpGet("real-time-stats")]
        public async Task<IActionResult> GetRealTimeStats()
        {
            try
            {
                var today = DateTime.Today;
                var thisWeek = today.AddDays(-(int)today.DayOfWeek);
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var stats = new {
                    Today = new {
                        NewCustomers = await _context.Customers.Include(c => c.User)
                            .Where(c => c.User.CreatedAt.Date == today).CountAsync(),
                        AppointmentsScheduled = await _context.Appointments
                            .Where(a => a.AppointmentDate.Date == today).CountAsync(),
                        AppointmentsCompleted = await _context.Appointments
                            .Where(a => a.CompletedAt.HasValue && a.CompletedAt.Value.Date == today).CountAsync()
                    },
                    ThisWeek = new {
                        NewCustomers = await _context.Customers.Include(c => c.User)
                            .Where(c => c.User.CreatedAt >= thisWeek).CountAsync(),
                        AppointmentsScheduled = await _context.Appointments
                            .Where(a => a.AppointmentDate >= thisWeek).CountAsync(),
                        Revenue = await _context.TreatmentPlans
                            .Where(tp => tp.PaymentStatus == "Paid" && tp.StartDate >= thisWeek)
                            .SumAsync(tp => tp.PaidAmount)
                    },
                    ThisMonth = new {
                        NewCustomers = await _context.Customers.Include(c => c.User)
                            .Where(c => c.User.CreatedAt >= thisMonth).CountAsync(),
                        CompletedTreatments = await _context.TreatmentPlans
                            .Where(tp => tp.Status == "Completed" && tp.EndDate >= thisMonth).CountAsync(),
                        Revenue = await _context.TreatmentPlans
                            .Where(tp => tp.PaymentStatus == "Paid" && tp.StartDate >= thisMonth)
                            .SumAsync(tp => tp.PaidAmount)
                    }
                };

                return Ok(new { 
                    success = true,
                    data = stats,
                    message = "Lấy thống kê thời gian thực thành công"
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
} 