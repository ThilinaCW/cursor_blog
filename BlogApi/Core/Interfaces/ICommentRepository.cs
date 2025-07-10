using BlogApi.Core.Entities;

namespace BlogApi.Core.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByBlogPostIdAsync(Guid blogPostId);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsOwnerAsync(Guid commentId, Guid userId);
    }
} 