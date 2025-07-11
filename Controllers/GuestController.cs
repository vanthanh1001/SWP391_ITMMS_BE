using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Models;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuestController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Trang chủ - Thông tin tổng quan về cơ sở y tế
        /// </summary>
        [HttpGet("home")]
        public async Task<IActionResult> GetHomeInfo()
        {
            try
            {
                var homeInfo = new {
                    clinicInfo = new {
                        name = "Trung tâm Điều trị Hiếm muộn ITMMS",
                        address = "123 Đường ABC, Quận 1, TP.HCM",
                        phone = "028-1234-5678",
                        email = "info@itmms.com",
                        workingHours = "Thứ 2 - Thứ 7: 7:00 - 17:00",
                        description = "Trung tâm chuyên về điều trị hiếm muộn với đội ngũ bác sĩ giàu kinh nghiệm và trang thiết bị hiện đại."
                    },
                    stats = new {
                        totalDoctors = await _context.Doctors.Where(d => d.IsAvailable).CountAsync(),
                        totalPatients = await _context.Customers.CountAsync(),
                        successfulTreatments = await _context.TreatmentPlans.Where(tp => tp.Status == "Completed").CountAsync(),
                        yearsOfExperience = 15
                    },
                    services = await _context.TreatmentServices
                        .Where(s => s.IsActive)
                        .Select(s => new {
                            s.Id,
                            s.Name,
                            s.Price,
                            Description = s.Description != null && s.Description.Length > 150 ? 
                                s.Description.Substring(0, 150) + "..." : s.Description
                        })
                        .Take(4)
                        .ToListAsync(),
                    featuredDoctors = await _context.Doctors
                        .Include(d => d.User)
                        .Where(d => d.IsAvailable)
                        .Select(d => new {
                            d.Id,
                            FullName = d.User.FirstName + " " + d.User.LastName,
                            d.Specialization,
                            d.ExperienceYears,
                            d.Education,
                            Description = d.Description != null && d.Description.Length > 100 ? 
                                d.Description.Substring(0, 100) + "..." : d.Description
                        })
                        .Take(3)
                        .ToListAsync()
                };

                return Ok(new { 
                    success = true,
                    data = homeInfo,
                    message = "Lấy thông tin trang chủ thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Danh sách tất cả dịch vụ điều trị (Public)
        /// </summary>
        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<object>>> GetServices()
        {
            var services = await _context.TreatmentServices
                .Include(s => s.Category)
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Price,
                    s.Duration,
                    Category = s.Category != null ? new
                    {
                        s.Category.Id,
                        s.Category.Name,
                        s.Category.Description
                    } : null,
                    s.CreatedAt
                })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(services);
        }

        /// <summary>
        /// Chi tiết dịch vụ điều trị (Public)
        /// </summary>
        [HttpGet("services/{id}")]
        public async Task<IActionResult> GetServiceDetails(int id)
        {
            try
            {
                var service = await _context.TreatmentServices
                    .Where(s => s.Id == id && s.IsActive)
                    .FirstOrDefaultAsync();

                if (service == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy dịch vụ" 
                    });
                }

                return Ok(new { 
                    success = true,
                    data = service,
                    message = "Lấy chi tiết dịch vụ thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Danh sách bác sĩ (Public)
        /// </summary>
        [HttpGet("doctors")]
        public async Task<ActionResult<IEnumerable<object>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsAvailable && d.User.IsActive)
                .Select(d => new
                {
                    d.Id,
                    FullName = $"{d.User.FirstName} {d.User.LastName}",
                    d.User.Email,
                    d.Specialization,
                    d.Education,
                    d.ExperienceYears,
                    d.Description,
                    d.ConsultationFee,
                    AverageRating = d.ReceivedFeedbacks.Any() ? 
                        d.ReceivedFeedbacks.Average(f => f.Rating) : 0
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>
        /// Chi tiết bác sĩ và lịch trình (Public)
        /// </summary>
        [HttpGet("doctors/{id}")]
        public async Task<IActionResult> GetDoctorDetails(int id)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.Id == id && d.IsAvailable)
                    .Select(d => new {
                        d.Id,
                        FullName = d.User.FirstName + " " + d.User.LastName,
                        d.Specialization,
                        d.LicenseNumber,
                        d.Education,
                        d.ExperienceYears,
                        d.Description,
                        d.ConsultationFee,
                        AverageRating = d.ReceivedFeedbacks.Any() ? d.ReceivedFeedbacks.Average(f => f.Rating) : 0,
                        TotalFeedbacks = d.ReceivedFeedbacks.Count(),
                        TotalPatients = d.Appointments.Select(a => a.CustomerId).Distinct().Count()
                    })
                    .FirstOrDefaultAsync();

                if (doctor == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy bác sĩ" 
                    });
                }

                return Ok(new { 
                    success = true,
                    data = doctor,
                    message = "Lấy thông tin bác sĩ thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Đánh giá của bác sĩ (Public)
        /// </summary>
        [HttpGet("doctors/{id}/reviews")]
        public async Task<IActionResult> GetDoctorReviews(int id)
        {
            try
            {
                var reviews = await _context.Feedbacks
                    .Include(f => f.Customer).ThenInclude(c => c.User)
                    .Where(f => f.DoctorId == id)
                    .OrderByDescending(f => f.CreatedAt)
                    .Select(f => new {
                        f.Id,
                        PatientName = f.Customer.User.FirstName + " " + f.Customer.User.LastName,
                        f.Rating,
                        f.Comment,
                        f.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = reviews,
                    message = "Lấy danh sách đánh giá thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Danh sách bài viết blog (Public)
        /// </summary>
        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<object>>> GetBlogs()
        {
            var blogs = await _context.BlogPosts
                .Include(b => b.Author)
                .Where(b => b.IsPublished)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    Author = new
                    {
                        b.Author.Id,
                        FullName = $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Author.Email
                    },
                    b.CreatedAt,
                    b.UpdatedAt
                })
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Ok(blogs);
        }

        /// <summary>
        /// Chi tiết bài viết blog (Public)
        /// </summary>
        [HttpGet("blogs/{id}")]
        public async Task<ActionResult<object>> GetBlog(int id)
        {
            var blog = await _context.BlogPosts
                .Include(b => b.Author)
                .Where(b => b.Id == id && b.IsPublished)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    Author = new
                    {
                        b.Author.Id,
                        FullName = $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Author.Email
                    },
                    b.CreatedAt,
                    b.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }

        /// <summary>
        /// Danh sách danh mục blog (Public)
        /// </summary>
        [HttpGet("blog/categories")]
        public async Task<IActionResult> GetBlogCategories()
        {
            try
            {
                var categories = await _context.BlogPosts
                    .Where(b => b.IsPublished)
                    .Select(b => b.Category)
                    .Distinct()
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = categories,
                    message = "Lấy danh sách danh mục thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Tìm kiếm (Public)
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] string type = "all")
        {
            try
            {
                var result = new Dictionary<string, object>();

                if (type == "all" || type == "doctors")
                {
                    var doctors = await _context.Doctors
                        .Include(d => d.User)
                        .Where(d => d.IsAvailable && 
                            (d.User.FirstName.Contains(keyword) || 
                             d.User.LastName.Contains(keyword) ||
                             d.Specialization.Contains(keyword)))
                        .Select(d => new {
                            d.Id,
                            FullName = d.User.FirstName + " " + d.User.LastName,
                            d.Specialization,
                            d.ExperienceYears
                        })
                        .ToListAsync();
                    result.Add("doctors", doctors);
                }

                if (type == "all" || type == "services")
                {
                    var services = await _context.TreatmentServices
                        .Where(s => s.IsActive && 
                            (s.Name.Contains(keyword) || 
                             s.Description.Contains(keyword)))
                        .Select(s => new {
                            s.Id,
                            s.Name,
                            s.Price
                        })
                        .ToListAsync();
                    result.Add("services", services);
                }

                if (type == "all" || type == "blog")
                {
                    var posts = await _context.BlogPosts
                        .Include(b => b.Author)
                        .Where(b => b.IsPublished && 
                            (b.Title.Contains(keyword) || 
                             b.Content.Contains(keyword) ||
                             b.Category.Contains(keyword)))
                        .Select(b => new {
                            b.Id,
                            b.Title,
                            b.Category,
                            AuthorName = b.Author.FirstName + " " + b.Author.LastName
                        })
                        .ToListAsync();
                    result.Add("blog", posts);
                }

                return Ok(new { 
                    success = true,
                    data = result,
                    message = "Tìm kiếm thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Câu hỏi thường gặp (Public)
        /// </summary>
        [HttpGet("faq")]
        public IActionResult GetFAQ()
        {
            try
            {
                var faqs = new List<object>
                {
                    new {
                        question = "Trung tâm có những phương pháp điều trị nào?",
                        answer = "Chúng tôi cung cấp nhiều phương pháp điều trị hiếm muộn hiện đại như IUI, IVF, ICSI, và các liệu pháp hormone."
                    },
                    new {
                        question = "Chi phí điều trị như thế nào?",
                        answer = "Chi phí sẽ phụ thuộc vào phương pháp điều trị cụ thể. Vui lòng liên hệ để được tư vấn chi tiết."
                    },
                    new {
                        question = "Làm thế nào để đặt lịch khám?",
                        answer = "Bạn có thể đặt lịch trực tuyến thông qua website hoặc gọi điện trực tiếp đến số hotline của trung tâm."
                    },
                    new {
                        question = "Tỷ lệ thành công của các phương pháp điều trị?",
                        answer = "Tỷ lệ thành công phụ thuộc vào nhiều yếu tố như tuổi tác, tình trạng sức khỏe. Trung bình đạt 60-70%."
                    },
                    new {
                        question = "Thời gian điều trị kéo dài bao lâu?",
                        answer = "Mỗi phương pháp có thời gian điều trị khác nhau, từ vài tuần đến vài tháng tùy trường hợp cụ thể."
                    }
                };

                return Ok(new { 
                    success = true,
                    data = faqs,
                    message = "Lấy danh sách FAQ thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Lỗi hệ thống: {ex.Message}" 
                });
            }
        }
    }
} 