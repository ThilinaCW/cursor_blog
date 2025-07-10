using Microsoft.EntityFrameworkCore;
using BlogApi.Core.Entities;
using BlogApi.Core.Interfaces;
using BlogApi.Infrastructure.Data;
using System.Text.Json;

namespace BlogApi.Infrastructure.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext _context;

        public BlogPostRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetPaginatedAsync(int page, int pageSize, string? search = null, bool onlyApproved = false)
        {
            var query = _context.BlogPosts.AsQueryable();
            query = query.Where(b => b.IsActive);
            if (onlyApproved)
            {
                query = query.Where(b => b.IsApproved);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.Contains(search) || 
                                        p.Content.Contains(search) || 
                                        p.Author.Contains(search));
            }
            return await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await _context.BlogPosts
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            blogPost.Id = Guid.NewGuid();
            blogPost.CreatedDate = DateTime.UtcNow;
            blogPost.IsActive = true;
            
            // Convert categories array to JSON string
            if (blogPost.CategoriesJson == null)
            {
                blogPost.CategoriesJson = "[]";
            }
            
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost> UpdateAsync(BlogPost blogPost)
        {
            blogPost.UpdatedDate = DateTime.UtcNow;
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
                return false;

            blogPost.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.BlogPosts.AnyAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<int> GetTotalCountAsync(string? search = null)
        {
            var query = _context.BlogPosts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.Contains(search) || 
                                        p.Content.Contains(search) || 
                                        p.Author.Contains(search));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var categories = new HashSet<string>();
            var blogPosts = await _context.BlogPosts
                .Where(b => b.IsActive && !string.IsNullOrEmpty(b.CategoriesJson))
                .Select(b => b.CategoriesJson)
                .ToListAsync();

            foreach (var categoriesJson in blogPosts)
            {
                try
                {
                    var categoryList = JsonSerializer.Deserialize<string[]>(categoriesJson ?? "[]");
                    if (categoryList != null)
                    {
                        foreach (var category in categoryList)
                        {
                            categories.Add(category);
                        }
                    }
                }
                catch
                {
                    // Skip invalid JSON
                }
            }

            return categories.OrderBy(c => c).ToList();
        }

        public async Task<IEnumerable<BlogPost>> GetByCategoryAsync(string category)
        {
            return await _context.BlogPosts
                .Where(b => b.IsActive && b.CategoriesJson.Contains(category))
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAuthorsAsync()
        {
            return await _context.BlogPosts
                .Where(b => b.IsActive)
                .Select(b => b.Author)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetByAuthorAsync(string author)
        {
            return await _context.BlogPosts
                .Where(b => b.Author == author && b.IsActive)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetFeaturedAsync()
        {
            return await _context.BlogPosts
                .Where(b => b.IsFeatured && b.IsActive)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
    }
} 