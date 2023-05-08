using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
            try
            {
                await DbSet.AddAsync(entity);
                Log.Information($"User with {entity.Id} was added.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await DbSet.AnyAsync(user => user.Email == email);
        }

        public async Task<string?> GetUserPasswordHashAsync(string email)
        {
            return (await DbSet
                .FirstOrDefaultAsync(user => user.Email == email))
                ?.PasswordHash;
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
    }
}
