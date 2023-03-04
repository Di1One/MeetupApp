using MeetupApp.Core.DataTransferObjects;

namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByRefreshTokenAsync(Guid token);
        Task<bool> IsUserExistsAsync(Guid userId);
        Task<bool> IsUserExistsAsync(string email);
        Task<bool> CheckUserPasswordAsync(string email, string password);
        Task<bool> CheckUserPasswordAsync(Guid userId, string password);
        Task<int> RegisterUserAsync(UserDto dto, string password);
    }
}
