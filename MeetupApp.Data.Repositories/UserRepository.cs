﻿using MeetupApp.Data.Abstractions.Repositories;
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

        public Task AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> FindBy(Expression<Func<User, bool>> searchExpression, params Expression<Func<User, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> Get()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
