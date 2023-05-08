using MeetupApp.DataBase.Entities;
using System.Linq.Expressions;

namespace MeetupApp.Data.Abstractions.Repositories
{
    public interface IUserRepository
    {
        //READ
        IQueryable<User> Get();

        Task<bool> IsUserExistsAsync(string email);
        Task<string?> GetUserPasswordHashAsync(string email);

        IQueryable<User> FindBy(Expression<Func<User, bool>> searchExpression,
            params Expression<Func<User, object>>[] includes);

        //CREATE
        Task AddAsync(User entity);
    }
}
