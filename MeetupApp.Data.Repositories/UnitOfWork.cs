using MeetupApp.Data.Abstractions;
using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;

namespace MeetupApp.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeetupAppDbContext _dbContext;
        public IRepository<User> Users { get; }
        public IRoleRepository Roles { get; }
        public IEventRepository Events { get; }
        public IRefreshTokenRepository RefreshToken { get; }

        public UnitOfWork(MeetupAppDbContext dbContext,
            IRepository<User> users,
            IRoleRepository roles,
            IRefreshTokenRepository refreshToken,
            IEventRepository events)
        {
            _dbContext = dbContext;
            Users = users;
            Roles = roles;
            RefreshToken = refreshToken;
            Events = events;
        }

        public async Task<int> Commit()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
