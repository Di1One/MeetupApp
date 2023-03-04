using MeetupApp.Core.DataTransferObjects;

namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IEventService
    {
        Task<EventDto> GetEventByIdAsync(Guid id);
        Task<List<EventDto>> GetEventsAsync();
        Task<int> CreateEventAsync(EventDto dto);
        Task<int> UpdateAsync(EventDto dto);
        Task<int> PatchEventAsync(Guid id, List<PatchModel> patchList);
        Task<int> DeleteEventAsync(EventDto dto);
    }
}
