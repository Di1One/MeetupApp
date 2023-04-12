using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeetupApp.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly MeetupAppDbContext Database;
        protected readonly DbSet<User> DbSet;

        public UserRepository(MeetupAppDbContext database)
        {
            Database = database;
            DbSet = database.Set<User>();
        }

        public async Task AddAsync(User entity)
        {
            await DbSet.AddAsync(entity);
        }

        public IQueryable<User> FindBy(Expression<Func<User, bool>> searchExpression, params Expression<Func<User, object>>[] includes)
        {
            var result = DbSet.Where(searchExpression);

            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) =>
                    current.Include(include));
            }

            return result;
        }

        public IQueryable<User> Get()
        {
            return DbSet;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id.Equals(id));
        }
    }
}
