using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;
using BlogApi.Core.Enums;
using System.Security.Claims;

namespace BlogApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserService userService, IBlogPostService blogPostService, ILogger<AdminController> logger)
        {
            _userService = userService;
            _blogPostService = blogPostService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
            try
            {
                _logger.LogInformation("Admin dashboard requested. User: {User}, Claims: {Claims}",
                    User.Identity?.Name,
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

                var pendingUsers = await _userService.GetPendingUsersAsync();
                var pendingPosts = await _blogPostService.GetPendingPostsAsync();
                var allUsers = await _userService.GetAllAsync();
                var allPosts = await _blogPostService.GetAllAsync();

                var dashboard = new AdminDashboardDto
                {
                    PendingUsers = pendingUsers.Count(),
                    PendingPosts = pendingPosts.Count(),
                    TotalUsers = allUsers.Count(),
                    TotalPosts = allPosts.Count()
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDashboard");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard data.", error = ex.Message });
            }
        }

        [HttpGet("users/pending")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetPendingUsers()
        {
            try
            {
                var pendingUsers = await _userService.GetPendingUsersAsync();
                return Ok(pendingUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving pending users.", error = ex.Message });
            }
        }

        [HttpPost("users/{userId}/approve")]
        public async Task<ActionResult> ApproveUser(Guid userId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _userService.ApproveUserAsync(userId, adminId);
                return Ok(new { message = "User approved successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving user.", error = ex.Message });
            }
        }

        [HttpPost("users/{userId}/reject")]
        public async Task<ActionResult> RejectUser(Guid userId, [FromBody] RejectUserDto rejectDto)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _userService.RejectUserAsync(userId, adminId, rejectDto.Reason);
                return Ok(new { message = "User rejected successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rejecting user.", error = ex.Message });
            }
        }

        [HttpGet("posts/pending")]
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetPendingPosts()
        {
            try
            {
                var pendingPosts = await _blogPostService.GetPendingPostsAsync();
                return Ok(pendingPosts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving pending posts.", error = ex.Message });
            }
        }

        [HttpPost("posts/{postId}/approve")]
        public async Task<ActionResult> ApprovePost(Guid postId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _blogPostService.ApprovePostAsync(postId, adminId);
                return Ok(new { message = "Post approved successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving post.", error = ex.Message });
            }
        }

        [HttpPost("posts/{postId}/reject")]
        public async Task<ActionResult> RejectPost(Guid postId, [FromBody] RejectPostDto rejectDto)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _blogPostService.RejectPostAsync(postId, adminId, rejectDto.Reason);
                return Ok(new { message = "Post rejected successfully." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rejecting post.", error = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token.");
            }
            return userId;
        }
    }
} 