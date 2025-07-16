using Microsoft.AspNetCore.Mvc;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "healthy", 
                message = "API is working!", 
                timestamp = DateTime.UtcNow 
            });
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new { 
                application = "ITMMS API",
                version = "1.0.0",
                environment = "Development",
                timestamp = DateTime.UtcNow
            });
        }
    }
} 