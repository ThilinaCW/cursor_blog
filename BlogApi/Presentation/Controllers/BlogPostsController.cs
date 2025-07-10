using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;
using BlogApi.Core.Enums;

namespace BlogApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<BlogPostDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 12, [FromQuery] string? search = null)
        {
            try
            {
                var paginated = await _blogPostService.GetPaginatedAsync(page, pageSize, search, onlyApproved: true);
                return Ok(paginated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving blog posts.", error = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<PaginatedResponseDto<BlogPostDto>>> GetAllPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 12, [FromQuery] string? search = null)
        {
            try
            {
                var paginated = await _blogPostService.GetPaginatedAsync(page, pageSize, search);
                return Ok(paginated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving blog posts.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPostDto>> GetById(Guid id)
        {
            try
            {
                var blogPost = await _blogPostService.GetByIdAsync(id);
                if (blogPost == null)
                    return NotFound(new { message = "Blog post not found." });

                return Ok(blogPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the blog post.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BlogPostDto>> Create([FromBody] CreateBlogPostDto createBlogPostDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createBlogPostDto.Title))
                    return BadRequest(new { message = "Title is required." });

                if (string.IsNullOrWhiteSpace(createBlogPostDto.Content))
                    return BadRequest(new { message = "Content is required." });

                if (string.IsNullOrWhiteSpace(createBlogPostDto.Author))
                    return BadRequest(new { message = "Author is required." });

                var blogPost = await _blogPostService.CreateAsync(createBlogPostDto);
                return CreatedAtAction(nameof(GetById), new { id = blogPost.Id }, blogPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the blog post.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BlogPostDto>> Update(Guid id, [FromBody] UpdateBlogPostDto updateBlogPostDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(updateBlogPostDto.Title))
                    return BadRequest(new { message = "Title is required." });

                if (string.IsNullOrWhiteSpace(updateBlogPostDto.Content))
                    return BadRequest(new { message = "Content is required." });

                if (string.IsNullOrWhiteSpace(updateBlogPostDto.Author))
                    return BadRequest(new { message = "Author is required." });

                var blogPost = await _blogPostService.UpdateAsync(id, updateBlogPostDto);
                return Ok(blogPost);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the blog post.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _blogPostService.DeleteAsync(id);
                if (!success)
                    return NotFound(new { message = "Blog post not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the blog post.", error = ex.Message });
            }
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetFeatured()
        {
            try
            {
                var blogPosts = await _blogPostService.GetFeaturedAsync();
                return Ok(blogPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving featured blog posts.", error = ex.Message });
            }
        }

        [HttpGet("categories/{category}")]
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetByCategory(string category)
        {
            try
            {
                var blogPosts = await _blogPostService.GetByCategoryAsync(category);
                return Ok(blogPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving blog posts by category.", error = ex.Message });
            }
        }

        [HttpGet("authors/{author}")]
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetByAuthor(string author)
        {
            try
            {
                var blogPosts = await _blogPostService.GetByAuthorAsync(author);
                return Ok(blogPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving blog posts by author.", error = ex.Message });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _blogPostService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories.", error = ex.Message });
            }
        }

        [HttpGet("authors")]
        public async Task<ActionResult<IEnumerable<string>>> GetAuthors()
        {
            try
            {
                var authors = await _blogPostService.GetAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving authors.", error = ex.Message });
            }
        }
    }
} 