namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IRoleService
    {
        Task<string> GetRoleNameByIdAsync(Guid id);
        Task<Guid?> GetRoleIdByNameAsync(string name);
    }
}
