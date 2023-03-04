using AutoMapper;
using MeetupApp.Core;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.Business.ServicesImplementations
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EventDto> GetEventByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Events.GetByIdAsync(id);

            if (entity == null)
                throw new ArgumentException("Failed to find record in the database that match the specified id. ", nameof(id));

            var dto = _mapper.Map<EventDto>(entity);
            return dto;
        }

        public async Task<List<EventDto>> GetEventsAsync()
        {
            var events = await _unitOfWork.Events.Get().ToListAsync();

            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<int> CreateEventAsync(EventDto dto)
        {
            var entity = _mapper.Map<Event>(dto);
            entity.Id = Guid.NewGuid();

            if (entity == null)
                throw new ArgumentException("Mapping EventDto to Event was not possible.", nameof(dto));

            await _unitOfWork.Events.AddAsync(entity);
            return await _unitOfWork.Commit();
        }

        public async Task<int> UpdateAsync(Guid id, EventDto dto)
        {
            var sourceDto = await GetEventByIdAsync(id);

            dto.Id = sourceDto.Id;
            dto.UserId = sourceDto.UserId;

            var entity = _mapper.Map<Event>(dto);

            if (entity == null)
                throw new ArgumentException("Mapping EventDto to Event was not possible.", nameof(dto));

            _unitOfWork.Events.Update(entity);
            return await _unitOfWork.Commit();
        }

        public async Task<int> PatchEventAsync(Guid id, List<PatchModel> patchList)
        {   
            await _unitOfWork.Events.PatchAsync(id, patchList);
            return await _unitOfWork.Commit();
        }

        public async Task<int> DeleteEventAsync(Guid id)
        {
            var entity = await _unitOfWork.Events.GetByIdAsync(id);

            if (entity != null)
            {
                _unitOfWork.Events.Remove(entity);
                return await _unitOfWork.Commit();
            }
            else
            {
                throw new ArgumentException("Event for removing doesn't exist.", nameof(id));
            }
        }

        public async Task<bool> IsEventExistAsync(string name)
        {
            return await _unitOfWork.Events.Get().AnyAsync(ev => ev.Name.Equals(name));
        }
    }
}
