using BlogApi.Core.Entities;
using BlogApi.Core.Interfaces;
using BlogApi.Core.DTOs;
using BlogApi.Core.Enums;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecret;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtSecret = configuration["Jwt:Key"] ?? "YourSuperSecretKeyHere12345678901234567890";
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            // Check if username or email already exists
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
                throw new ArgumentException("Username already exists.");

            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
                throw new ArgumentException("Email already exists.");

            var user = new User
            {
                Username = createUserDto.Username.Trim(),
                Email = createUserDto.Email.Trim().ToLower(),
                PasswordHash = HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName.Trim(),
                LastName = createUserDto.LastName.Trim(),
                Bio = createUserDto.Bio?.Trim() ?? string.Empty
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return MapToDto(createdUser);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException($"User with ID {id} not found.");

            user.FirstName = updateUserDto.FirstName.Trim();
            user.LastName = updateUserDto.LastName.Trim();
            user.Bio = updateUserDto.Bio?.Trim() ?? string.Empty;
            user.ProfileImageUrl = updateUserDto.ProfileImageUrl?.Trim() ?? string.Empty;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated.");

            // Update last login
            await _userRepository.UpdateLastLoginAsync(user.Id);

            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ssZ");

            return new LoginResponseDto
            {
                Token = token,
                User = MapToDto(user),
                ExpiresAt = expiresAt
            };
        }

        public async Task<bool> CheckUsernameAvailabilityAsync(string username)
        {
            return !await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> CheckEmailAvailabilityAsync(string email)
        {
            return !await _userRepository.EmailExistsAsync(email);
        }

        public async Task UpdateLastLoginAsync(Guid userId)
        {
            await _userRepository.UpdateLastLoginAsync(userId);
        }

        public async Task<bool> ApproveUserAsync(Guid userId, Guid adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            user.IsApproved = true;
            user.ApprovedDate = DateTime.UtcNow;
            user.ApprovedBy = adminId;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> RejectUserAsync(Guid userId, Guid adminId, string reason)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            user.IsApproved = false;
            user.IsActive = false; // Deactivate rejected user

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetPendingUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => !u.IsApproved && u.IsActive).Select(MapToDto);
        }

        public async Task<IEnumerable<UserDto>> GetApprovedUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => u.IsApproved && u.IsActive).Select(MapToDto);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "BlogApi",
                Audience = "BlogApiUsers",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfileImageUrl = user.ProfileImageUrl,
                Role = user.Role,
                IsActive = user.IsActive,
                IsApproved = user.IsApproved,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                ApprovedDate = user.ApprovedDate,
                ApprovedBy = user.ApprovedBy
            };
        }
    }
} 