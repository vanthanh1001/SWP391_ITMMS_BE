using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Services;
using System.Security.Claims;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class FileUploadController : ControllerBase
    {
        private readonly PublitioService _publitioService;

        public FileUploadController(PublitioService publitioService)
        {
            _publitioService = publitioService;
        }

        [HttpGet("health")]
        [AllowAnonymous] // Health check doesn't need auth
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
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string? category = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { 
                    success = false,
                    message = "No file uploaded" 
                });
            }

            // Get user info from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = User.FindFirst("FirstName")?.Value;
            var lastName = User.FindFirst("LastName")?.Value;

            if (userIdClaim == null || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { 
                    success = false,
                    message = "Invalid authentication token" 
                });
            }

            try
            {
                // Create meaningful filename with user context
                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var fileExtension = Path.GetExtension(file.FileName);
                var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                
                // Include user role and category in filename for organization
                var categoryPrefix = string.IsNullOrEmpty(category) ? "general" : category.ToLower();
                var rolePrefix = userRole.ToLower();
                var newFileName = $"{rolePrefix}_{categoryPrefix}_{timestamp}_{originalName}{fileExtension}";

                // Create a copy of the file with new name
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var renamedFile = new FormFile(stream, 0, stream.Length, file.Name, newFileName)
                {
                    Headers = file.Headers,
                    ContentType = file.ContentType
                };

                var fileUrl = await _publitioService.UploadFileAsync(renamedFile);
                
                return Ok(new { 
                    success = true,
                    message = "File uploaded successfully!",
                    fileUrl = fileUrl,
                    originalFileName = file.FileName,
                    savedFileName = newFileName,
                    fileSize = file.Length,
                    contentType = file.ContentType,
                    uploadedBy = new {
                        userId = userIdClaim.Value,
                        role = userRole,
                        name = $"{firstName} {lastName}",
                        email = userEmail
                    },
                    category = categoryPrefix,
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

        [HttpPost("medical-records")]
        public async Task<IActionResult> UploadMedicalRecord(IFormFile file, [FromQuery] int? patientId = null, [FromQuery] int? appointmentId = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { 
                    success = false,
                    message = "No file uploaded" 
                });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            // Only doctors can upload medical records
            if (userRole != "Doctor" && userRole != "Admin")
            {
                return Forbid("Only doctors can upload medical records");
            }

            return await Upload(file, $"medical_records_patient_{patientId}_appointment_{appointmentId}");
        }

        [HttpPost("profile-images")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { 
                    success = false,
                    message = "No file uploaded" 
                });
            }

            // Validate file type for profile images
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new {
                    success = false,
                    message = "Only image files (JPEG, PNG, GIF) are allowed for profile pictures"
                });
            }

            return await Upload(file, "profile_image");
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

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Only allow admins or the file owner to delete files
            // Note: In a real system, you'd check file ownership in database
            if (userRole != "Admin")
            {
                return Forbid("Only administrators or file owners can delete files");
            }

            try
            {
                await _publitioService.DeleteFileAsync(fileId);
                return Ok(new { 
                    success = true,
                    message = "File deleted successfully",
                    deletedBy = new {
                        userId = userId,
                        role = userRole
                    },
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

        [HttpGet("my-files")]
        public async Task<IActionResult> GetMyFiles()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Note: This is a placeholder. In a real system, you'd:
            // 1. Store file metadata in database with owner info
            // 2. Query files by user ID
            // 3. Return user's files only

            return Ok(new {
                success = true,
                message = "Feature not implemented yet",
                note = "File metadata should be stored in database to enable this feature",
                user = new {
                    userId = userId,
                    role = userRole
                }
            });
        }
    }
} 