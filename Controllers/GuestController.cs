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
                            s.ServiceName,
                            s.ServiceCode,
                            s.BasePrice,
                            s.SuccessRate,
                            Description = s.Description.Length > 150 ? 
                                s.Description.Substring(0, 150) + "..." : s.Description
                        })
                        .Take(4)
                        .ToListAsync(),
                    featuredDoctors = await _context.Doctors
                        .Include(d => d.User)
                        .Where(d => d.IsAvailable)
                        .Select(d => new {
                            d.Id,
                            DoctorName = d.User.FullName,
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
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var services = await _context.TreatmentServices
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.ServiceName)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = services,
                    message = "Lấy danh sách dịch vụ thành công"
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
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.IsAvailable)
                    .Select(d => new {
                        d.Id,
                        DoctorName = d.User.FullName,
                        d.Specialization,
                        d.ExperienceYears,
                        d.Education,
                        d.Description,
                        d.ConsultationFee,
                        AverageRating = d.ReceivedFeedbacks.Any() ? d.ReceivedFeedbacks.Average(f => f.Rating) : 0,
                        TotalFeedbacks = d.ReceivedFeedbacks.Count()
                    })
                    .OrderByDescending(d => d.ExperienceYears)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = doctors,
                    message = "Lấy danh sách bác sĩ thành công"
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
                        DoctorName = d.User.FullName,
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
                        f.Rating,
                        f.Comment,
                        f.CreatedAt,
                        CustomerName = f.Customer.User.FullName,
                        // Ẩn một phần tên để bảo mật
                        MaskedCustomerName = f.Customer.User.FullName.Length > 2 ? 
                            f.Customer.User.FullName.Substring(0, 1) + "***" + 
                            f.Customer.User.FullName.Substring(f.Customer.User.FullName.Length - 1) : 
                            "***"
                    })
                    .Take(10)
                    .ToListAsync();

                var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

                return Ok(new { 
                    success = true,
                    data = new {
                        averageRating = Math.Round(averageRating, 1),
                        totalReviews = reviews.Count,
                        reviews
                    },
                    message = "Lấy đánh giá bác sĩ thành công"
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
        /// Bài viết blog công khai
        /// </summary>
        [HttpGet("blog")]
        public async Task<IActionResult> GetPublicBlogPosts([FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.BlogPosts
                    .Include(bp => bp.Author)
                    .Where(bp => bp.IsPublished);

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(bp => bp.Category == category);
                }

                var totalPosts = await query.CountAsync();
                var posts = await query
                    .OrderByDescending(bp => bp.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(bp => new {
                        bp.Id,
                        bp.Title,
                        bp.Category,
                        bp.CreatedAt,
                        AuthorName = bp.Author.FullName,
                        // Excerpt - 200 ký tự đầu
                        Excerpt = bp.Content.Length > 200 ? 
                            bp.Content.Substring(0, 200) + "..." : bp.Content
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = new {
                        posts,
                        pagination = new {
                            currentPage = page,
                            pageSize,
                            totalPosts,
                            totalPages = (int)Math.Ceiling((double)totalPosts / pageSize)
                        }
                    },
                    message = "Lấy danh sách blog thành công"
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
        /// Chi tiết bài viết blog
        /// </summary>
        [HttpGet("blog/{id}")]
        public async Task<IActionResult> GetBlogPost(int id)
        {
            try
            {
                var post = await _context.BlogPosts
                    .Include(bp => bp.Author)
                    .Where(bp => bp.Id == id && bp.IsPublished)
                    .Select(bp => new {
                        bp.Id,
                        bp.Title,
                        bp.Content,
                        bp.Category,
                        bp.CreatedAt,
                        bp.UpdatedAt,
                        AuthorName = bp.Author.FullName,
                        AuthorRole = bp.Author.Role
                    })
                    .FirstOrDefaultAsync();

                if (post == null)
                {
                    return NotFound(new { 
                        success = false,
                        message = "Không tìm thấy bài viết" 
                    });
                }

                return Ok(new { 
                    success = true,
                    data = post,
                    message = "Lấy chi tiết bài viết thành công"
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
        /// Danh mục blog
        /// </summary>
        [HttpGet("blog/categories")]
        public async Task<IActionResult> GetBlogCategories()
        {
            try
            {
                var categories = await _context.BlogPosts
                    .Where(bp => bp.IsPublished)
                    .GroupBy(bp => bp.Category)
                    .Select(g => new {
                        Category = g.Key,
                        PostCount = g.Count()
                    })
                    .OrderByDescending(c => c.PostCount)
                    .ToListAsync();

                return Ok(new { 
                    success = true,
                    data = categories,
                    message = "Lấy danh mục blog thành công"
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
        /// Tìm kiếm (dịch vụ, bác sĩ, blog)
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] string type = "all")
        {
            try
            {
                var result = new {
                    keyword,
                    services = type == "all" || type == "services" ? 
                        await _context.TreatmentServices
                            .Where(s => s.IsActive && 
                                (s.ServiceName.Contains(keyword) || (s.Description != null && s.Description.Contains(keyword))))
                            .Select(s => new {
                                s.Id,
                                s.ServiceName,
                                s.ServiceCode,
                                s.BasePrice,
                                Description = s.Description != null && s.Description.Length > 100 ? 
                                    s.Description.Substring(0, 100) + "..." : s.Description
                            })
                            .Take(5)
                            .ToListAsync() : null,
                    doctors = type == "all" || type == "doctors" ?
                        await _context.Doctors
                            .Include(d => d.User)
                            .Where(d => d.IsAvailable && 
                                (d.User.FullName.Contains(keyword) || 
                                 d.Specialization.Contains(keyword) ||
                                 (d.Description != null && d.Description.Contains(keyword))))
                            .Select(d => new {
                                d.Id,
                                DoctorName = d.User.FullName,
                                d.Specialization,
                                d.ExperienceYears,
                                Description = d.Description != null && d.Description.Length > 100 ? 
                                    d.Description.Substring(0, 100) + "..." : d.Description
                            })
                            .Take(5)
                            .ToListAsync() : null,
                    blogs = type == "all" || type == "blogs" ?
                        await _context.BlogPosts
                            .Include(bp => bp.Author)
                            .Where(bp => bp.IsPublished && 
                                (bp.Title.Contains(keyword) || bp.Content.Contains(keyword)))
                            .Select(bp => new {
                                bp.Id,
                                bp.Title,
                                bp.Category,
                                bp.CreatedAt,
                                AuthorName = bp.Author.FullName,
                                Excerpt = bp.Content.Length > 150 ? 
                                    bp.Content.Substring(0, 150) + "..." : bp.Content
                            })
                            .Take(5)
                            .ToListAsync() : null
                };

                return Ok(new { 
                    success = true,
                    data = result,
                    message = $"Tìm kiếm '{keyword}' thành công"
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
        /// FAQ - Câu hỏi thường gặp
        /// </summary>
        [HttpGet("faq")]
        public IActionResult GetFAQ()
        {
            var faqs = new List<object>
            {
                new {
                    question = "Điều trị hiếm muộn có đau không?",
                    answer = "Các thủ thuật điều trị hiếm muộn hiện đại thường không gây đau đáng kể. Bác sĩ sẽ sử dụng gây tê cục bộ hoặc an thần nhẹ khi cần thiết."
                },
                new {
                    question = "Tỷ lệ thành công của IVF là bao nhiêu?",
                    answer = "Tỷ lệ thành công của IVF phụ thuộc vào nhiều yếu tố như tuổi, nguyên nhân hiếm muộn. Trung bình tại trung tâm chúng tôi là 65-70%."
                },
                new {
                    question = "Chi phí điều trị hiếm muộn là bao nhiêu?",
                    answer = "Chi phí phụ thuộc vào phương pháp điều trị. IUI từ 15 triệu, IVF từ 85 triệu. Chúng tôi có các gói ưu đãi và hỗ trợ tài chính."
                },
                new {
                    question = "Thời gian điều trị hiếm muộn mất bao lâu?",
                    answer = "IUI thường mất 2 tuần, IVF khoảng 4-6 tuần. Tuy nhiên có thể cần nhiều chu kỳ điều trị để đạt kết quả tốt nhất."
                },
                new {
                    question = "Có cần nghỉ ngơi lâu sau điều trị không?",
                    answer = "Sau IUI có thể hoạt động bình thường ngay. Sau IVF nên nghỉ ngơi 1-2 ngày và tránh hoạt động nặng trong 1 tuần."
                }
            };

            return Ok(new { 
                success = true,
                data = faqs,
                message = "Lấy FAQ thành công"
            });
        }
    }
} 