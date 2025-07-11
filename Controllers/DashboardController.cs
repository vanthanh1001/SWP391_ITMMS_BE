using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalServices = await _context.TreatmentServices.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();

            var pendingAppointments = await _context.Appointments
                .Where(a => a.Status == "Pending")
                .CountAsync();

            var completedAppointments = await _context.Appointments
                .Where(a => a.Status == "Completed")
                .CountAsync();

            var totalRevenue = await _context.TreatmentPlans
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.TotalCost);

            var recentAppointments = await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .Select(a => new
                {
                    a.Id,
                    CustomerName = $"{a.Customer.User.FirstName} {a.Customer.User.LastName}",
                    DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                    a.AppointmentDate,
                    a.Status,
                    a.Type
                })
                .ToListAsync();

            return Ok(new
            {
                totalCustomers,
                totalDoctors,
                totalServices,
                totalAppointments,
                pendingAppointments,
                completedAppointments,
                totalRevenue,
                recentAppointments
            });
        }

        [HttpGet("doctor/{doctorId}/stats")]
        public async Task<ActionResult<object>> GetDoctorStats(int doctorId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }

            var totalAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .CountAsync();

            var pendingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == "Pending")
                .CountAsync();

            var completedAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == "Completed")
                .CountAsync();

            var averageRating = await _context.Feedbacks
                .Where(f => f.DoctorId == doctorId)
                .AverageAsync(f => (double?)f.Rating) ?? 0;

            var recentAppointments = await _context.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .Select(a => new
                {
                    a.Id,
                    CustomerName = $"{a.Customer.User.FirstName} {a.Customer.User.LastName}",
                    a.AppointmentDate,
                    a.Status,
                    a.Type
                })
                .ToListAsync();

            return Ok(new
            {
                doctorName = $"{doctor.User.FirstName} {doctor.User.LastName}",
                totalAppointments,
                pendingAppointments,
                completedAppointments,
                averageRating,
                recentAppointments
            });
        }
    }
} 