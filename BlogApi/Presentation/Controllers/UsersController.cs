using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;

namespace BlogApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user.", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createUserDto.Username))
                    return BadRequest(new { message = "Username is required." });

                if (string.IsNullOrWhiteSpace(createUserDto.Email))
                    return BadRequest(new { message = "Email is required." });

                if (string.IsNullOrWhiteSpace(createUserDto.Password))
                    return BadRequest(new { message = "Password is required." });

                if (string.IsNullOrWhiteSpace(createUserDto.FirstName))
                    return BadRequest(new { message = "First name is required." });

                if (string.IsNullOrWhiteSpace(createUserDto.LastName))
                    return BadRequest(new { message = "Last name is required." });

                var user = await _userService.CreateAsync(createUserDto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user.", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Username))
                    return BadRequest(new { message = "Username is required." });

                if (string.IsNullOrWhiteSpace(loginDto.Password))
                    return BadRequest(new { message = "Password is required." });

                var response = await _userService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid currentUserId) || currentUserId != id)
                    return Forbid();

                if (string.IsNullOrWhiteSpace(updateUserDto.FirstName))
                    return BadRequest(new { message = "First name is required." });

                if (string.IsNullOrWhiteSpace(updateUserDto.LastName))
                    return BadRequest(new { message = "Last name is required." });

                var user = await _userService.UpdateAsync(id, updateUserDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid currentUserId) || currentUserId != id)
                    return Forbid();

                var success = await _userService.DeleteAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user.", error = ex.Message });
            }
        }

        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<object>> CheckUsernameAvailability(string username)
        {
            try
            {
                var isAvailable = await _userService.CheckUsernameAvailabilityAsync(username);
                return Ok(new { exists = !isAvailable });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking username availability.", error = ex.Message });
            }
        }

        [HttpGet("check-email/{email}")]
        public async Task<ActionResult<object>> CheckEmailAvailability(string email)
        {
            try
            {
                var isAvailable = await _userService.CheckEmailAvailabilityAsync(email);
                return Ok(new { exists = !isAvailable });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking email availability.", error = ex.Message });
            }
        }
    }
} 