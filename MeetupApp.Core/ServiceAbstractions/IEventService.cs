﻿using MeetupApp.Core.DataTransferObjects;

namespace MeetupApp.Core.ServiceAbstractions
{
    public interface IEventService
    {
        Task<EventDto> GetEventByIdAsync(Guid id);
        Task<List<EventDto>> GetEventsAsync();
        Task<int> CreateEventAsync(EventDto dto);
        Task<int> UpdateAsync(Guid id, EventDto dto);
        Task<int> PatchEventAsync(Guid id, EventDto dto);
        Task<int> DeleteEventAsync(Guid id);
        Task<bool> IsEventExistAsync(string name);
    }
}
