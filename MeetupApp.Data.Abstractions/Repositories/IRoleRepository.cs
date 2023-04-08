using MeetupApp.DataBase.Entities;
using System.Linq.Expressions;

namespace MeetupApp.Data.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        IQueryable<Role> FindByRoleName(Expression<Func<Role, bool>> searchExpression,
           params Expression<Func<Role, object>>[] includes);
    }
}
