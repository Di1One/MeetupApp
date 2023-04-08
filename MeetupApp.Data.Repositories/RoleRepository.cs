using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeetupApp.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        protected readonly MeetupAppDbContext Database;
        protected readonly DbSet<Role> DbSet;

        public RoleRepository(MeetupAppDbContext database)
        {
            Database = database;
            DbSet = database.Set<Role>();
        }

        public IQueryable<Role> FindByRoleName(Expression<Func<Role, bool>> searchExpression,
            params Expression<Func<Role, object>>[] includes)
        {
            var result = DbSet.Where(searchExpression);

            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) =>
                    current.Include(include));
            }

            return result;
        }
    }
}
