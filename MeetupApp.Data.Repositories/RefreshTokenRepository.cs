using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MeetupApp.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        protected readonly MeetupAppDbContext Database;
        protected readonly DbSet<RefreshToken> DbSet;

        public RefreshTokenRepository(MeetupAppDbContext database)
        {
            Database = database;
            DbSet = database.Set<RefreshToken>();
        }

        public async Task AddTokenAsync(RefreshToken entity)
        {
            try
            {
                await DbSet.AddAsync(entity);
                Log.Information($"Refresh token with {entity.Id} was added.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }

        // Gets IQueryable object
        public IQueryable<RefreshToken> GetAllToken()
        {
            return DbSet;
        }

        public void RemoveToken(RefreshToken entity)
        {
            try
            {
                DbSet.Remove(entity);
                Log.Information($"Refresh token with {entity.Id} was deleted.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }
    }
}
