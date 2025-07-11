using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SWP391_ITMMS_Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Customer";

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Customer? Customer { get; set; }

        [JsonIgnore]
        public virtual Doctor? Doctor { get; set; }

        [JsonIgnore]
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
} 