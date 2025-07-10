using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;

namespace BlogApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("post/{blogPostId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByBlogPostId(Guid blogPostId)
        {
            try
            {
                Guid? currentUserId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(userIdClaim, out Guid userId))
                    {
                        currentUserId = userId;
                    }
                }

                var comments = await _commentService.GetByBlogPostIdAsync(blogPostId, currentUserId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving comments.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetById(Guid id)
        {
            try
            {
                Guid? currentUserId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(userIdClaim, out Guid userId))
                    {
                        currentUserId = userId;
                    }
                }

                var comment = await _commentService.GetByIdAsync(id, currentUserId);
                if (comment == null)
                    return NotFound(new { message = "Comment not found." });

                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the comment.", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createCommentDto.Content))
                    return BadRequest(new { message = "Comment content is required." });

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid user token." });

                var comment = await _commentService.CreateAsync(createCommentDto, userId);
                return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the comment.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<CommentDto>> Update(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(updateCommentDto.Content))
                    return BadRequest(new { message = "Comment content is required." });

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid user token." });

                var comment = await _commentService.UpdateAsync(id, updateCommentDto, userId);
                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the comment.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid user token." });

                var success = await _commentService.DeleteAsync(id, userId);
                if (!success)
                    return NotFound(new { message = "Comment not found." });

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the comment.", error = ex.Message });
            }
        }
    }
} 