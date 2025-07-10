using BlogApi.Core.DTOs;

namespace BlogApi.Core.Interfaces
{
    public interface IBlogPostService
    {
        Task<IEnumerable<BlogPostDto>> GetAllAsync();
        Task<PaginatedResponseDto<BlogPostDto>> GetPaginatedAsync(int page = 1, int pageSize = 10, string? search = null, bool onlyApproved = false);
        Task<BlogPostDto?> GetByIdAsync(Guid id);
        Task<BlogPostDto> CreateAsync(CreateBlogPostDto createBlogPostDto);
        Task<BlogPostDto> UpdateAsync(Guid id, UpdateBlogPostDto updateBlogPostDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<BlogPostDto>> GetFeaturedAsync();
        Task<IEnumerable<BlogPostDto>> GetByCategoryAsync(string category);
        Task<IEnumerable<BlogPostDto>> GetByAuthorAsync(string author);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<string>> GetAuthorsAsync();
        
        // Admin methods
        Task<bool> ApprovePostAsync(Guid postId, Guid adminId);
        Task<bool> RejectPostAsync(Guid postId, Guid adminId, string reason);
        Task<IEnumerable<BlogPostDto>> GetPendingPostsAsync();
        Task<IEnumerable<BlogPostDto>> GetApprovedPostsAsync();
    }
} 