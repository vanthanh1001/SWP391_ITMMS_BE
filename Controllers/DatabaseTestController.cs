using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DatabaseTestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return StatusCode(500, new { 
                        success = false, 
                        message = "Cannot connect to database" 
                    });
                }

                // Get some basic info
                var usersCount = await _context.Users.CountAsync();
                var doctorsCount = await _context.Doctors.CountAsync();
                var appointmentsCount = await _context.Appointments.CountAsync();

                return Ok(new {
                    success = true,
                    message = "Database connection successful!",
                    database = "ITMMS_DB",
                    statistics = new {
                        usersCount = usersCount,
                        doctorsCount = doctorsCount,
                        appointmentsCount = appointmentsCount
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "Database connection failed",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("tables")]
        public async Task<IActionResult> GetTableInfo()
        {
            try
            {
                var tables = new List<object>();

                // Check if tables exist and get counts
                try { tables.Add(new { table = "Users", count = await _context.Users.CountAsync() }); } catch { tables.Add(new { table = "Users", count = "Error" }); }
                try { tables.Add(new { table = "Doctors", count = await _context.Doctors.CountAsync() }); } catch { tables.Add(new { table = "Doctors", count = "Error" }); }
                try { tables.Add(new { table = "Customers", count = await _context.Customers.CountAsync() }); } catch { tables.Add(new { table = "Customers", count = "Error" }); }
                try { tables.Add(new { table = "Appointments", count = await _context.Appointments.CountAsync() }); } catch { tables.Add(new { table = "Appointments", count = "Error" }); }
                try { tables.Add(new { table = "BlogPosts", count = await _context.BlogPosts.CountAsync() }); } catch { tables.Add(new { table = "BlogPosts", count = "Error" }); }

                return Ok(new {
                    success = true,
                    message = "Table information retrieved",
                    tables = tables,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "Failed to get table information",
                    error = ex.Message
                });
            }
        }
    }
} 