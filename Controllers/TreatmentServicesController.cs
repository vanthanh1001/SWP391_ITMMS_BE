using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreatmentServicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreatmentService>>> GetServices()
        {
            return await _context.TreatmentServices
                .Include(s => s.Category)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TreatmentService>> GetService(int id)
        {
            var service = await _context.TreatmentServices
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        [HttpPost]
        public async Task<ActionResult<TreatmentService>> CreateService(TreatmentService service)
        {
            if (service.CategoryId == 0)
            {
                return BadRequest("CategoryId is required");
            }

            service.CreatedAt = DateTime.UtcNow;
            _context.TreatmentServices.Add(service);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == service.CategoryId))
                {
                    return BadRequest("Invalid CategoryId");
                }
                throw;
            }

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, TreatmentService service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            var existingService = await _context.TreatmentServices.FindAsync(id);
            if (existingService == null)
            {
                return NotFound();
            }

            existingService.Name = service.Name;
            existingService.Description = service.Description;
            existingService.Price = service.Price;
            existingService.Duration = service.Duration;
            existingService.CategoryId = service.CategoryId;
            existingService.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == service.CategoryId))
                {
                    return BadRequest("Invalid CategoryId");
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.TreatmentServices.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.TreatmentServices.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(int id)
        {
            return _context.TreatmentServices.Any(e => e.Id == id);
        }
    }
} 