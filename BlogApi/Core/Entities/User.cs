using System.ComponentModel.DataAnnotations;
using BlogApi.Core.Enums;

namespace BlogApi.Core.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Bio { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string ProfileImageUrl { get; set; } = string.Empty;
        
        public UserRole Role { get; set; } = UserRole.User;
        
        public bool IsActive { get; set; } = true;
        
        public bool IsApproved { get; set; } = false;
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? LastLoginDate { get; set; }
        
        public DateTime? ApprovedDate { get; set; }
        
        public Guid? ApprovedBy { get; set; }
    }
} 