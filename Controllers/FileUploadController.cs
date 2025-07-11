using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;

        public FileUploadController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            try
            {
                var fileUrl = await _firebaseService.UploadFileAsync(file);
                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Filename is required");
            }

            try
            {
                await _firebaseService.DeleteFileAsync(fileName);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 