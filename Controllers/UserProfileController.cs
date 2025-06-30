using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Data;
using System.Collections.Generic;
using System.Linq;
using System;

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

        // Lấy hồ sơ người dùng
        [HttpGet("{userId}")]
        public ActionResult<User> GetUserProfile(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Sửa hồ sơ người dùng
        [HttpPut("{userId}")]
        public ActionResult<User> UpdateUserProfile(int userId, [FromBody] UserUpdateDto updateDto)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();
            user.Username = updateDto.Username;
            user.Password = updateDto.Password;
            user.Email = updateDto.Email;
            user.FullName = updateDto.FullName;
            user.Phone = updateDto.Phone;
            user.Address = updateDto.Address;
            user.DateOfBirth = updateDto.DateOfBirth.Date;
            _context.SaveChanges();
            return Ok(user);
        }

        // Lấy lịch sử điều trị của người dùng
        [HttpGet("{userId}/treatment-history")]
        public ActionResult<IEnumerable<TreatmentHistory>> GetTreatmentHistory(int userId)
        {
            var histories = _context.TreatmentHistories.Where(t => t.UserId == userId).ToList();
            return Ok(histories);
        }

        // Thêm lịch sử điều trị cho user
        [HttpPost("{userId}/treatment-history")]
        public IActionResult AddTreatmentHistory(int userId, [FromBody] TreatmentHistoryCreateDto dto)
        {
            var history = new TreatmentHistory
            {
                UserId = userId,
                Description = dto.Description,
                Date = DateTime.UtcNow
            };
            _context.TreatmentHistories.Add(history);
            _context.SaveChanges();
            return Ok(history);
        }
    }
} 