using MeetupApp.Core;
using MeetupApp.Data.Abstractions;
using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

        public async Task<bool> IsEventExistAsync(string name)
        {
            return await DbSet.AnyAsync(ev => ev.Name == name);
        }

        public async Task AddEventAsync(Event entity)
        {
            try
            {
                await DbSet.AddAsync(entity);
                Log.Information($"Event with {entity.Id} was added.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }

        public void Update(Event entity)
        {
            try
            {
                DbSet.Update(entity);
                Log.Information($"Event with {entity.Id} was updated.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }

        public async Task PatchAsync(Guid id, List<PatchModel> patchData)
        {
            try
            {
                var model = await DbSet.FirstOrDefaultAsync(entity => entity.Id.Equals(id));

                var nameValuePropertiesPairs = patchData
                    .ToDictionary(
                        patchModel => patchModel.PropertyName,
                        patchModel => patchModel.PropertyValue);

                var dbEntityEntry = Database.Entry(model);
                dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
                dbEntityEntry.State = EntityState.Modified;

                Log.Information($"Event with {id} was edited.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }

        public void RemoveEvent(Event entity)
        {
            try
            {
                DbSet.Remove(entity);
                Log.Information($"Event with {entity.Id} was deleted.");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                throw;
            }
        }

    }
}
