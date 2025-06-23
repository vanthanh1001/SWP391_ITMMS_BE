using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_ITMMS_Api.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        
        public int AuthorId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [StringLength(50)]
        public string Category { get; set; } // Health Tips, Treatment Info, Success Stories, etc.
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; } = false;
        
        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }
    }
} 