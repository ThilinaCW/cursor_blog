using System.ComponentModel.DataAnnotations;

namespace BlogApi.Core.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public Guid BlogPostId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual BlogPost BlogPost { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
} 