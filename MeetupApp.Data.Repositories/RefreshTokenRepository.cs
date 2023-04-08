using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await DbSet.AddAsync(entity);
        }

        // Gets IQueryable object
        public IQueryable<RefreshToken> GetAllToken()
        {
            return DbSet;
        }

        public void RemoveToken(RefreshToken entity)
        {
            DbSet.Remove(entity);
        }
    }
}
