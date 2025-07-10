using BlogApi.Core.DTOs;

namespace BlogApi.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(Guid id);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> CheckUsernameAvailabilityAsync(string username);
        Task<bool> CheckEmailAvailabilityAsync(string email);
        Task UpdateLastLoginAsync(Guid userId);
        
        // Admin methods
        Task<bool> ApproveUserAsync(Guid userId, Guid adminId);
        Task<bool> RejectUserAsync(Guid userId, Guid adminId, string reason);
        Task<IEnumerable<UserDto>> GetPendingUsersAsync();
        Task<IEnumerable<UserDto>> GetApprovedUsersAsync();
    }
} 