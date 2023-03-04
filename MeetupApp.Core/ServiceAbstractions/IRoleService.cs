namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IRoleService
    {
        Task<Guid> GetRoleIdForDefaultRoleAsync();
    }
}
