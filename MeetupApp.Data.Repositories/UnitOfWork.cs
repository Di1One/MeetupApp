using MeetupApp.Data.Abstractions;
using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;

namespace MeetupApp.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeetupAppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IEventRepository Events { get; }
        public IRefreshTokenRepository RefreshToken { get; }

        public UnitOfWork(MeetupAppDbContext dbContext,
            IUserRepository users,
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
