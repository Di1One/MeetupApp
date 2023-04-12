using MeetupApp.DataBase.Entities;
using System.Linq.Expressions;

namespace MeetupApp.Data.Abstractions.Repositories
{
    public interface IUserRepository
    {
        //READ
        Task<User> GetByIdAsync(Guid id);
        IQueryable<User> Get();

        IQueryable<User> FindBy(Expression<Func<User, bool>> searchExpression,
            params Expression<Func<User, object>>[] includes);

        //CREATE
        Task AddAsync(User entity);
    }
}
