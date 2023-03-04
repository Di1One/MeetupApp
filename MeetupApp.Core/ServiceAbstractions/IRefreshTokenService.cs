namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IRefreshTokenService
    {
        Task<int> CreateRefreshTokenAsync(Guid tokenValue, Guid userId);
        Task<int> RemoveRefreshTokenAsync(Guid tokenValue);
    }
}
