using BlogApi.Core.Entities;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;

namespace BlogApi.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<CommentDto>> GetByBlogPostIdAsync(Guid blogPostId, Guid? currentUserId = null)
        {
            var comments = await _commentRepository.GetByBlogPostIdAsync(blogPostId);
            return comments.Select(c => MapToDto(c, currentUserId));
        }

        public async Task<CommentDto?> GetByIdAsync(Guid id, Guid? currentUserId = null)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return comment != null ? MapToDto(comment, currentUserId) : null;
        }

        public async Task<CommentDto> CreateAsync(CreateCommentDto createCommentDto, Guid userId)
        {
            try
            {
                var comment = new Comment
                {
                    Content = createCommentDto.Content.Trim(),
                    BlogPostId = createCommentDto.BlogPostId,
                    UserId = userId,
                    IsActive = true
                };

                var createdComment = await _commentRepository.CreateAsync(comment);
                
                if (createdComment.User == null)
                {
                    throw new InvalidOperationException("User navigation property is null after creating comment");
                }
                
                return MapToDto(createdComment, userId);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine($"Error creating comment: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<CommentDto> UpdateAsync(Guid id, UpdateCommentDto updateCommentDto, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new ArgumentException($"Comment with ID {id} not found.");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You can only edit your own comments.");

            comment.Content = updateCommentDto.Content.Trim();
            var updatedComment = await _commentRepository.UpdateAsync(comment);
            return MapToDto(updatedComment, userId);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            if (!await _commentRepository.IsOwnerAsync(id, userId))
                throw new UnauthorizedAccessException("You can only delete your own comments.");

            return await _commentRepository.DeleteAsync(id);
        }

        private static CommentDto MapToDto(Comment comment, Guid? currentUserId)
        {
            if (comment.User == null)
            {
                throw new InvalidOperationException($"User navigation property is null for comment ID {comment.Id}");
            }

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                BlogPostId = comment.BlogPostId,
                UserId = comment.UserId,
                UserName = comment.User.Username,
                UserFirstName = comment.User.FirstName,
                UserLastName = comment.User.LastName,
                CreatedDate = comment.CreatedDate,
                UpdatedDate = comment.UpdatedDate,
                IsActive = comment.IsActive,
                CanEdit = currentUserId.HasValue && comment.UserId == currentUserId.Value
            };
        }
    }
} 