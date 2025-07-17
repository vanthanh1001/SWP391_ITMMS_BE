using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Services;
using System.Security.Claims;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public UserController(IUserService userService, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                // Get current users ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return BadRequest("Invalid user ID");

                // Get user information
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                // Return user profile without sensitive information
                var profile = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    fullName = user.FullName,
                    phone = user.Phone,
                    address = user.Address,
                    avatarUrl = user.AvatarUrl,
                    role = user.Role,
                    createdAt = user.CreatedAt,
                    updatedAt = user.UpdatedAt
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user profile", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Get current users ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();

                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return BadRequest("Invalid user ID");

                // Upload image to Cloudinary
                var imageUrl = await _cloudinaryService.UploadImageAsync(file);

                // Update user's avatar URL in database
                var success = await _userService.UpdateAvatarUrlAsync(userId, imageUrl);
                if (!success)
                    return NotFound("User not found");

                return Ok(new { avatarUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading avatar", error = ex.Message });
            }
        }
    }
} 