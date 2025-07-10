using BlogApi.Core.Entities;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;
using System.Text.Json;

namespace BlogApi.Application.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogPostService(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<IEnumerable<BlogPostDto>> GetAllAsync()
        {
            var blogPosts = await _blogPostRepository.GetAllAsync();
            return blogPosts.Select(MapToDto);
        }

        public async Task<BlogPostDto?> GetByIdAsync(Guid id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            return blogPost != null ? MapToDto(blogPost) : null;
        }

        public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto createBlogPostDto)
        {
            var blogPost = new BlogPost
            {
                Title = createBlogPostDto.Title.Trim(),
                Content = createBlogPostDto.Content.Trim(),
                Author = createBlogPostDto.Author.Trim(),
                ImageUrl = createBlogPostDto.ImageUrl?.Trim(),
                IsFeatured = createBlogPostDto.IsFeatured,
                CategoriesJson = JsonSerializer.Serialize(createBlogPostDto.Categories ?? Array.Empty<string>())
            };

            var createdBlogPost = await _blogPostRepository.CreateAsync(blogPost);
            return MapToDto(createdBlogPost);
        }

        public async Task<BlogPostDto> UpdateAsync(Guid id, UpdateBlogPostDto updateBlogPostDto)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
                throw new ArgumentException($"Blog post with ID {id} not found.");

            blogPost.Title = updateBlogPostDto.Title.Trim();
            blogPost.Content = updateBlogPostDto.Content.Trim();
            blogPost.Author = updateBlogPostDto.Author.Trim();
            blogPost.ImageUrl = updateBlogPostDto.ImageUrl?.Trim();
            blogPost.IsFeatured = updateBlogPostDto.IsFeatured;
            blogPost.CategoriesJson = JsonSerializer.Serialize(updateBlogPostDto.Categories ?? Array.Empty<string>());

            var updatedBlogPost = await _blogPostRepository.UpdateAsync(blogPost);
            return MapToDto(updatedBlogPost);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _blogPostRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BlogPostDto>> GetFeaturedAsync()
        {
            var blogPosts = await _blogPostRepository.GetFeaturedAsync();
            return blogPosts.Select(MapToDto);
        }

        public async Task<IEnumerable<BlogPostDto>> GetByCategoryAsync(string category)
        {
            var blogPosts = await _blogPostRepository.GetByCategoryAsync(category);
            return blogPosts.Select(MapToDto);
        }

        public async Task<IEnumerable<BlogPostDto>> GetByAuthorAsync(string author)
        {
            var blogPosts = await _blogPostRepository.GetByAuthorAsync(author);
            return blogPosts.Select(MapToDto);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _blogPostRepository.GetCategoriesAsync();
        }

        public async Task<IEnumerable<string>> GetAuthorsAsync()
        {
            return await _blogPostRepository.GetAuthorsAsync();
        }

        public async Task<PaginatedResponseDto<BlogPostDto>> GetPaginatedAsync(int page = 1, int pageSize = 10, string? search = null, bool onlyApproved = false)
        {
            var allPosts = await _blogPostRepository.GetPaginatedAsync(page, pageSize, search, onlyApproved);
            var filtered = allPosts;
            var totalCount = allPosts.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new PaginatedResponseDto<BlogPostDto>
            {
                Data = filtered.Select(MapToDto),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                HasMore = page < totalPages
            };
        }

        public async Task<bool> ApprovePostAsync(Guid postId, Guid adminId)
        {
            var post = await _blogPostRepository.GetByIdAsync(postId);
            if (post == null)
                throw new ArgumentException($"Blog post with ID {postId} not found.");

            post.IsApproved = true;
            post.ApprovedDate = DateTime.UtcNow;
            post.ApprovedBy = adminId;

            await _blogPostRepository.UpdateAsync(post);
            return true;
        }

        public async Task<bool> RejectPostAsync(Guid postId, Guid adminId, string reason)
        {
            var post = await _blogPostRepository.GetByIdAsync(postId);
            if (post == null)
                throw new ArgumentException($"Blog post with ID {postId} not found.");

            post.IsApproved = false;
            post.RejectionReason = reason;

            await _blogPostRepository.UpdateAsync(post);
            return true;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPendingPostsAsync()
        {
            var posts = await _blogPostRepository.GetAllAsync();
            return posts.Where(p => !p.IsApproved && p.IsActive).Select(MapToDto);
        }

        public async Task<IEnumerable<BlogPostDto>> GetApprovedPostsAsync()
        {
            var posts = await _blogPostRepository.GetAllAsync();
            return posts.Where(p => p.IsApproved && p.IsActive).Select(MapToDto);
        }

        private static BlogPostDto MapToDto(BlogPost blogPost)
        {
            string[] categories = Array.Empty<string>();
            try
            {
                if (!string.IsNullOrEmpty(blogPost.CategoriesJson))
                {
                    categories = JsonSerializer.Deserialize<string[]>(blogPost.CategoriesJson) ?? Array.Empty<string>();
                }
            }
            catch
            {
                // If JSON deserialization fails, use empty array
                categories = Array.Empty<string>();
            }

            return new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ImageUrl = blogPost.ImageUrl,
                IsFeatured = blogPost.IsFeatured,
                Categories = categories,
                CreatedDate = blogPost.CreatedDate,
                UpdatedDate = blogPost.UpdatedDate,
                IsActive = blogPost.IsActive,
                IsApproved = blogPost.IsApproved,
                ApprovedDate = blogPost.ApprovedDate,
                ApprovedBy = blogPost.ApprovedBy,
                RejectionReason = blogPost.RejectionReason
            };
        }
    }
} 