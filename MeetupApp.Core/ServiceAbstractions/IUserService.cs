using MeetupApp.Core.DataTransferObjects;

namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByRefreshTokenAsync(Guid token);
        Task<bool> IsUserExists(Guid userId);
        Task<bool> IsUserExists(string email);
        Task<bool> CheckUserPassword(string email, string password);
        Task<bool> CheckUserPassword(Guid userId, string password);
        Task<int> RegisterUser(UserDto dto, string password);
    }
}
