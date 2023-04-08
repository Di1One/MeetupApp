using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase.Entities;

namespace MeetupApp.Data.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRoleRepository Roles { get; }
        IRepository<Event> Events { get; }
        IRefreshTokenRepository RefreshToken { get; }
        Task<int> Commit();
    }
}
