using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Models.DTOs;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile(int id, UserUpdateDto updateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.Phone = updateDto.Phone;
            user.Address = updateDto.Address;
            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
} 