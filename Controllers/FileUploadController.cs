using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly PublitioService _publitioService;

        public FileUploadController(PublitioService publitioService)
        {
            _publitioService = publitioService;
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "healthy", 
                message = "FileUpload API is working!", 
                timestamp = DateTime.UtcNow,
                publitioConfigured = _publitioService != null
            });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { 
                    success = false,
                    message = "No file uploaded" 
                });
            }

            try
            {
                var fileUrl = await _publitioService.UploadFileAsync(file);
                return Ok(new { 
                    success = true,
                    message = "File uploaded successfully!",
                    fileUrl = fileUrl,
                    fileName = file.FileName,
                    fileSize = file.Length,
                    contentType = file.ContentType,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "Upload failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                return BadRequest(new { 
                    success = false,
                    message = "File ID is required" 
                });
            }

            try
            {
                await _publitioService.DeleteFileAsync(fileId);
                return Ok(new { 
                    success = true,
                    message = "File deleted successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "Delete failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
} 