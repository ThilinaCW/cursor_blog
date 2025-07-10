using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BlogApi.Core.Entities
{
    public class BlogPost
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsFeatured { get; set; } = false;
        
        public string? CategoriesJson { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsApproved { get; set; } = false;
        
        public DateTime? ApprovedDate { get; set; }
        
        public Guid? ApprovedBy { get; set; }
        
        public string? RejectionReason { get; set; }
        
        // Navigation property for easy access to categories
        [NotMapped]
        public List<string> Categories
        {
            get => JsonSerializer.Deserialize<List<string>>(CategoriesJson) ?? new List<string>();
            set => CategoriesJson = JsonSerializer.Serialize(value);
        }
    }
} 