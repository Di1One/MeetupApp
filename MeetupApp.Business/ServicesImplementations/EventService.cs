using AutoMapper;
using MeetupApp.Core;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

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
            var entity = await _unitOfWork.Events.GetEventByIdAsync(id);

            if (entity == null)
                throw new ArgumentException("Failed to find record in the database that match the specified id. ", nameof(id));

            var dto = _mapper.Map<EventDto>(entity);
            return dto;
        }

        public async Task<List<EventDto>> GetEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllEvent().ToListAsync();

            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<int> CreateEventAsync(EventDto dto)
        {
            var entity = _mapper.Map<Event>(dto);
            entity.Id = Guid.NewGuid();

            if (entity == null)
                throw new ArgumentException("Mapping EventDto to Event was not possible.", nameof(dto));

            await _unitOfWork.Events.AddEventAsync(entity);
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
            var entity = await _unitOfWork.Events.GetEventByIdAsync(id);

            try
            {
                if (entity != null)
                {
                    _unitOfWork.Events.RemoveEvent(entity);
                    return await _unitOfWork.Commit();
                }
                else
                {
                    throw new ArgumentException("Event for removing doesn't exist.", nameof(id));
                }
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return 0;
            }
            catch(Exception ex) 
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return 0;
            }
        }

        public async Task<bool> IsEventExistAsync(string name)
        {
            return await _unitOfWork.Events.GetAllEvent().AnyAsync(ev => ev.Name.Equals(name));
        }
    }
}
