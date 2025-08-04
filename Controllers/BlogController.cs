using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Models;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/blog")]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public BlogController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // DTO classes
        public class CreateBlogPostDto
        {
            [Required]
            [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
            public required string Title { get; set; }

            [Required]
            [MinLength(50, ErrorMessage = "Nội dung phải có ít nhất 50 ký tự")]
            public required string Content { get; set; }

            [StringLength(50)]
            public required string Category { get; set; } = "Health Tips";

            public bool IsPublished { get; set; } = false;
        }

        public class UpdateBlogPostDto
        {
            [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
            public string? Title { get; set; }

            [MinLength(50, ErrorMessage = "Nội dung phải có ít nhất 50 ký tự")]
            public string? Content { get; set; }

            [StringLength(50)]
            public string? Category { get; set; }

            public bool? IsPublished { get; set; }
        }

        public class BlogPostResponseDto
        {
            public int Id { get; set; }
            public required string Title { get; set; }
            public required string Content { get; set; }
            public required string Category { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsPublished { get; set; }
            public int AuthorId { get; set; }
        }

        // CREATE - POST: /api/blog
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDto dto)
        {
            try
            {
                // Validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                // Lấy token từ header
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { 
                        success = false, 
                        message = "Không tìm thấy token xác thực" 
                    });
                }

                var token = authHeader.Substring("Bearer ".Length);
                var userId = _jwtService.GetUserIdFromToken(token);

                if (!userId.HasValue)
                {
                    return Unauthorized(new { 
                        success = false, 
                        message = "Token không hợp lệ" 
                    });
                }

                // Kiểm tra user có tồn tại và là Doctor không
                var author = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.Role == "Doctor");

                if (author == null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Chỉ bác sĩ mới có thể tạo bài viết blog" 
                    });
                }

                // Tạo blog post mới
                var blogPost = new BlogPost
                {
                    AuthorId = author.Id,
                    Title = dto.Title,
                    Content = dto.Content,
                    Category = dto.Category,
                    IsPublished = dto.IsPublished,
                    CreatedAt = DateTime.Now
                };

                // Lưu vào database
                _context.BlogPosts.Add(blogPost);
                await _context.SaveChangesAsync();

                // Trả về response
                var response = new BlogPostResponseDto
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Content = blogPost.Content,
                    Category = blogPost.Category,
                    CreatedAt = blogPost.CreatedAt,
                    UpdatedAt = blogPost.UpdatedAt,
                    IsPublished = blogPost.IsPublished,
                    AuthorId = blogPost.AuthorId
                };

                return CreatedAtAction(nameof(GetBlogPostById), new { id = blogPost.Id }, new { 
                    success = true, 
                    message = "Tạo bài viết thành công",
                    data = response 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }

        // READ - GET: /api/blog (Lấy danh sách bài viết)
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts([FromQuery] string? category, [FromQuery] bool? isPublished)
        {
            try
            {
                var query = _context.BlogPosts
                    .Include(bp => bp.Author)
                    .AsQueryable();

                // Filter theo category
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(bp => bp.Category == category);
                }

                // Filter theo trạng thái published
                if (isPublished.HasValue)
                {
                    query = query.Where(bp => bp.IsPublished == isPublished.Value);
                }

                var blogPosts = await query
                    .OrderByDescending(bp => bp.CreatedAt)
                    .Select(bp => new BlogPostResponseDto
                    {
                        Id = bp.Id,
                        Title = bp.Title,
                        Content = bp.Content,
                        Category = bp.Category,
                        CreatedAt = bp.CreatedAt,
                        UpdatedAt = bp.UpdatedAt,
                        IsPublished = bp.IsPublished,
                        AuthorId = bp.AuthorId
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    message = "Lấy danh sách bài viết thành công",
                    data = blogPosts,
                    totalCount = blogPosts.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }

        // READ - GET: /api/blog/{id} (Lấy chi tiết bài viết)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPostById(int id)
        {
            try
            {
                var blogPost = await _context.BlogPosts
                    .Include(bp => bp.Author)
                    .FirstOrDefaultAsync(bp => bp.Id == id);

                if (blogPost == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy bài viết" 
                    });
                }

                var response = new BlogPostResponseDto
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Content = blogPost.Content,
                    Category = blogPost.Category,
                    CreatedAt = blogPost.CreatedAt,
                    UpdatedAt = blogPost.UpdatedAt,
                    IsPublished = blogPost.IsPublished,
                    AuthorId = blogPost.AuthorId
                };

                return Ok(new { 
                    success = true, 
                    message = "Lấy thông tin bài viết thành công",
                    data = response 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }

        // UPDATE - PUT: /api/blog/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogPost(int id, [FromBody] UpdateBlogPostDto dto)
        {
            try
            {
                var blogPost = await _context.BlogPosts
                    .Include(bp => bp.Author)
                    .FirstOrDefaultAsync(bp => bp.Id == id);

                if (blogPost == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy bài viết" 
                    });
                }

                // Kiểm tra quyền chỉnh sửa
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { success = false, message = "Không tìm thấy token xác thực" });
                }
                var token = authHeader.Substring("Bearer ".Length);
                var currentUserId = _jwtService.GetUserIdFromToken(token);
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Token không hợp lệ" });
                }
                if (blogPost.AuthorId != currentUserId)
                {
                    return Forbid();
                }

                // Cập nhật thông tin
                if (!string.IsNullOrEmpty(dto.Title))
                    blogPost.Title = dto.Title;

                if (!string.IsNullOrEmpty(dto.Content))
                    blogPost.Content = dto.Content;

                if (!string.IsNullOrEmpty(dto.Category))
                    blogPost.Category = dto.Category;

                if (dto.IsPublished.HasValue)
                    blogPost.IsPublished = dto.IsPublished.Value;

                blogPost.UpdatedAt = DateTime.Now;

                _context.BlogPosts.Update(blogPost);
                await _context.SaveChangesAsync();

                var response = new BlogPostResponseDto
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Content = blogPost.Content,
                    Category = blogPost.Category,
                    CreatedAt = blogPost.CreatedAt,
                    UpdatedAt = blogPost.UpdatedAt,
                    IsPublished = blogPost.IsPublished,
                    AuthorId = blogPost.AuthorId
                };

                return Ok(new { 
                    success = true, 
                    message = "Cập nhật bài viết thành công",
                    data = response 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }

        // DELETE - DELETE: /api/blog/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            try
            {
                var blogPost = await _context.BlogPosts.FindAsync(id);

                if (blogPost == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy bài viết" 
                    });
                }

                // Kiểm tra quyền xóa
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { success = false, message = "Không tìm thấy token xác thực" });
                }
                var token = authHeader.Substring("Bearer ".Length);
                var currentUserId = _jwtService.GetUserIdFromToken(token);
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Token không hợp lệ" });
                }
                if (blogPost.AuthorId != currentUserId)
                {
                    return Forbid();
                }

                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Xóa bài viết thành công" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }

        // GET: /api/blog/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchBlogPosts(
            [FromQuery] string? keyword,
            [FromQuery] string? category,
            [FromQuery] bool? isPublished,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.BlogPosts
                    .Include(bp => bp.Author)
                    .AsQueryable();

                // Tìm kiếm theo keyword
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(bp => 
                        bp.Title.Contains(keyword) || 
                        bp.Content.Contains(keyword));
                }

                // Lọc theo category
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(bp => bp.Category == category);
                }

                // Lọc theo trạng thái published
                if (isPublished.HasValue)
                {
                    query = query.Where(bp => bp.IsPublished == isPublished.Value);
                }

                // Tính tổng số bài viết
                var totalCount = await query.CountAsync();

                // Phân trang
                var blogPosts = await query
                    .OrderByDescending(bp => bp.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(bp => new BlogPostResponseDto
                    {
                        Id = bp.Id,
                        Title = bp.Title,
                        Content = bp.Content,
                        Category = bp.Category,
                        CreatedAt = bp.CreatedAt,
                        UpdatedAt = bp.UpdatedAt,
                        IsPublished = bp.IsPublished,
                        AuthorId = bp.AuthorId
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    message = "Lấy danh sách bài viết thành công",
                    data = blogPosts,
                    pagination = new {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }

        // GET: /api/blog/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _context.BlogPosts
                    .Select(bp => bp.Category)
                    .Distinct()
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = categories 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi hệ thống: " + ex.Message 
                });
            }
        }
    }
} 