using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected User? CurrentUser => HttpContext.Items["User"] as User;
        protected int? CurrentUserId => HttpContext.Items["UserId"] as int?;

        protected bool IsAuthenticated => CurrentUser != null;

        protected IActionResult UnauthorizedResponse(string message = "Bạn cần đăng nhập để thực hiện hành động này")
        {
            return Unauthorized(new { message });
        }

        protected IActionResult ForbiddenResponse(string message = "Bạn không có quyền thực hiện hành động này")
        {
            return StatusCode(403, new { message });
        }

        protected IActionResult SuccessResponse(object data, string message = "Thành công")
        {
            return Ok(new { message, data });
        }

        protected IActionResult ErrorResponse(string message = "Có lỗi xảy ra", int statusCode = 500)
        {
            return StatusCode(statusCode, new { message });
        }
    }
} 