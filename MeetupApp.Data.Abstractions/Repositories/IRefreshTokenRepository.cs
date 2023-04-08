using MeetupApp.DataBase.Entities;

namespace MeetupApp.Data.Abstractions.Repositories
{
    public interface IRefreshTokenRepository
    {
        //READ
        IQueryable<RefreshToken> GetAllToken();
        //CREATE
        Task AddTokenAsync(RefreshToken entity);
        //DELETE
        void RemoveToken(RefreshToken entity);
    }
}
