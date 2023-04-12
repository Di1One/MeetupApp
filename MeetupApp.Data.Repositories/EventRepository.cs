﻿using MeetupApp.Core;
using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        protected readonly MeetupAppDbContext Database;
        protected readonly DbSet<Event> DbSet;

        public EventRepository(MeetupAppDbContext database)
        {
            Database = database;
            DbSet = database.Set<Event>();
        }

        public async Task<Event> GetEventByIdAsync(Guid id)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id.Equals(id));
        }

        public IQueryable<Event> GetAllEvent()
        {
            return DbSet;
        }

        public async Task AddEventAsync(Event entity)
        {
            await DbSet.AddAsync(entity);
        }

        public void Update(Event entity)
        {
            DbSet.Update(entity);
        }

        public async Task PatchAsync(Guid id, List<PatchModel> patchData)
        {
            var model = DbSet.FirstOrDefaultAsync(entity => entity.Id.Equals(id));

            var nameValuePropertiesPairs = patchData
                .ToDictionary(
                    patchModel => patchModel.PropertyName,
                    patchModel => patchModel.PropertyValue);

            var dbEntityEntry = Database.Entry(model);
            dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
            dbEntityEntry.State = EntityState.Modified;
        }

        public void RemoveEvent(Event entity)
        {
            DbSet.Remove(entity);
        }

    }
}