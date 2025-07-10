using BlogApi.Core.DTOs;

namespace BlogApi.Core.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetByBlogPostIdAsync(Guid blogPostId, Guid? currentUserId = null);
        Task<CommentDto?> GetByIdAsync(Guid id, Guid? currentUserId = null);
        Task<CommentDto> CreateAsync(CreateCommentDto createCommentDto, Guid userId);
        Task<CommentDto> UpdateAsync(Guid id, UpdateCommentDto updateCommentDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
} 