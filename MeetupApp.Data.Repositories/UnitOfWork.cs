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
        public IRepository<Role> Roles { get; }
        public IRepository<RefreshToken> RefreshToken { get; }

        public UnitOfWork(MeetupAppDbContext dbContext, 
            IRepository<User> users,
            IRepository<Role> roles,
            IRepository<RefreshToken> refreshToken)
        {
            _dbContext = dbContext;
            Users = users;
            Roles = roles;
            RefreshToken = refreshToken;
        }

        public async Task<int> Commit()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
