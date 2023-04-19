using MeetupApp.Core.DataTransferObjects;
using MeetupApp.WebAPI.Models.Responces;

namespace MeetupApp.WebAPI.Utils
{
    public interface IJwtUtil
    {
        Task<TokenResponseModel> GenerateTokenAsync(UserDto dto);
        Task<bool> RemoveRefreshTokenAsync(Guid requestRefreshToken);
    }
}
