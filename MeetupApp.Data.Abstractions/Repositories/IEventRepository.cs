using MeetupApp.Core;
using MeetupApp.DataBase.Entities;

namespace MeetupApp.Data.Abstractions.Repositories
{
    public interface IEventRepository
    {
        //READ
        Task<Event> GetEventByIdAsync(Guid id);
        IQueryable<Event> GetAllEvent();
        Task<bool> IsEventExistAsync(string name);

        //CREATE
        Task AddEventAsync(Event entity);

        //UPDATE
        void Update(Event entity);
        Task PatchAsync(Guid id, List<PatchModel> patchData);

        //DELETE
        void RemoveEvent(Event entity);
    }
}
