using MeetupApp.Core.DataTransferObjects;

namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByRefreshTokenAsync(Guid token);
        Task<bool> CheckUserPasswordAsync(string email, string password);
        Task<(bool success, string message)> RegisterUserAsync(UserDto dto, string password);
        Task<(bool success, string message)> AuthenticateUserAsync(string email, string password);
    }
}
