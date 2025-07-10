using Microsoft.EntityFrameworkCore;
using BlogApi.Core.Entities;
using BlogApi.Core.Interfaces;
using BlogApi.Infrastructure.Data;

namespace BlogApi.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogDbContext _context;

        public CommentRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetByBlogPostIdAsync(Guid blogPostId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.BlogPostId == blogPostId && c.IsActive)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            comment.Id = Guid.NewGuid();
            comment.CreatedDate = DateTime.UtcNow;
            comment.IsActive = true;
            
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            
            // Reload the comment with User navigation property
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id) ?? comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            comment.UpdatedDate = DateTime.UtcNow;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            
            // Reload the comment with User navigation property
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id) ?? comment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return false;

            comment.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<bool> IsOwnerAsync(Guid commentId, Guid userId)
        {
            return await _context.Comments.AnyAsync(c => c.Id == commentId && c.UserId == userId && c.IsActive);
        }
    }
} 