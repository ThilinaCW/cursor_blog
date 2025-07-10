namespace BlogApi.Core.DTOs
{
    public class BlogPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public string[] Categories { get; set; } = Array.Empty<string>();
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class CreateBlogPostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public string[] Categories { get; set; } = Array.Empty<string>();
    }

    public class UpdateBlogPostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public string[] Categories { get; set; } = Array.Empty<string>();
    }

    public class PaginatedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }
} 