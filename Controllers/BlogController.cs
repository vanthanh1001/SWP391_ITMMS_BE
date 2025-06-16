using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/blog")]
    public class BlogController : ControllerBase
    {
        // Giả lập database bằng static list
        private static List<BlogPost> _posts = new List<BlogPost>();

        // GET: /api/blog
        [HttpGet]
        public ActionResult<IEnumerable<BlogPost>> GetAll()
        {
            return Ok(_posts);
        }

        // GET: /api/blog/{id}
        [HttpGet("{id}")]
        public ActionResult<BlogPost> GetById(int id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        // POST: /api/blog/create
        [HttpPost("create")]
        public IActionResult Create([FromBody] BlogPost post)
        {
            post.Id = _posts.Count > 0 ? _posts.Max(p => p.Id) + 1 : 1;
            post.CreatedAt = DateTime.Now;
            _posts.Add(post);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        // PUT: /api/blog/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] BlogPost updatedPost)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return NotFound();
            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;
            post.UpdatedAt = DateTime.Now;
            return NoContent();
        }

        // DELETE: /api/blog/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return NotFound();
            _posts.Remove(post);
            return NoContent();
        }
    }
} 