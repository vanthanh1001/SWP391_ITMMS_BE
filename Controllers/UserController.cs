using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Services;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var success = await _userService.UpdateUserAsync(user);
            if (!success)
            {
                return NotFound(new { message = "User not found" });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound(new { message = "User not found" });
            }

            return NoContent();
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<bool>> CheckEmailAvailability(string email, [FromQuery] int? excludeUserId = null)
        {
            var isUnique = await _userService.IsEmailUniqueAsync(email, excludeUserId);
            return Ok(isUnique);
        }
    }
} 