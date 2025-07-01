using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using System.Security.Claims;

namespace SWP391_ITMMS_Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected User? CurrentUser => GetCurrentUserFromClaims();
        protected int? CurrentUserId => GetCurrentUserIdFromClaims();

        protected bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

        private User? GetCurrentUserFromClaims()
        {
            if (!IsAuthenticated) return null;

            var userId = GetCurrentUserIdFromClaims();
            if (!userId.HasValue) return null;

            // DEBUG: Log all claims
            Console.WriteLine("=== JWT CLAIMS DEBUG ===");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            Console.WriteLine("=======================");

            // Tạo User object từ claims
            var user = new User
            {
                Id = userId.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? "",
                Username = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("unique_name")?.Value ?? "",
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value ?? "",
                FullName = User.FindFirst("FullName")?.Value ?? ""
            };

            // Thêm Customer nếu có CustomerId claim
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;
            if (!string.IsNullOrEmpty(customerIdClaim) && int.TryParse(customerIdClaim, out int customerId))
            {
                user.Customer = new Customer { Id = customerId, UserId = userId.Value };
            }

            // Thêm Doctor nếu có DoctorId claim
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;
            if (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
            {
                user.Doctor = new Doctor { Id = doctorId, UserId = userId.Value };
            }

            return user;
        }

        private int? GetCurrentUserIdFromClaims()
        {
            if (!IsAuthenticated) return null;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("nameid")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        protected IActionResult UnauthorizedResponse(string message = "Bạn cần đăng nhập để thực hiện hành động này")
        {
            return Unauthorized(new { message });
        }

        protected IActionResult ForbiddenResponse(string message = "Bạn không có quyền thực hiện hành động này")
        {
            return StatusCode(403, new { message });
        }

        protected IActionResult SuccessResponse(object? data, string message = "Thành công")
        {
            return Ok(new { message, data });
        }

        protected IActionResult ErrorResponse(string message = "Có lỗi xảy ra", int statusCode = 500)
        {
            return StatusCode(statusCode, new { message });
        }
    }
} 