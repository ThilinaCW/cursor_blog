using BlogApi.Core.Entities;

namespace BlogApi.Core.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task<IEnumerable<BlogPost>> GetPaginatedAsync(int page, int pageSize, string? search = null, bool onlyApproved = false);
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<BlogPost> CreateAsync(BlogPost blogPost);
        Task<BlogPost> UpdateAsync(BlogPost blogPost);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> GetTotalCountAsync(string? search = null);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<BlogPost>> GetByCategoryAsync(string category);
        Task<IEnumerable<string>> GetAuthorsAsync();
        Task<IEnumerable<BlogPost>> GetByAuthorAsync(string author);
        Task<IEnumerable<BlogPost>> GetFeaturedAsync();
    }
} 