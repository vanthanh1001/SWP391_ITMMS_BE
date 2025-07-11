using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SWP391_ITMMS_Api.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AuthorId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Summary { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        public string? FeaturedImage { get; set; }
        
        public string? Tags { get; set; }
        
        public bool IsPublished { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual User Author { get; set; } = null!;
    }
} 