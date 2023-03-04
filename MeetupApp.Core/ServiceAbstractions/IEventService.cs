using MeetupApp.Core.DataTransferObjects;

namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IEventService
    {
        Task<EventDto> GetByIdAsync(Guid id);
        Task<List<EventDto>> GetEventsAsync();
        Task<int> CreateEventAsync(EventDto dto);
        Task<int> UpdateAsync(EventDto dto);
        Task<int> PatcheEventAsync(Guid id, List<PatchModel> patchList);
        Task<int> DeleteEventAsync(EventDto dto);
    }
}
