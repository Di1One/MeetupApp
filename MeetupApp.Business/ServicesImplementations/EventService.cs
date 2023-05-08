using AutoMapper;
using MeetupApp.Core;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace MeetupApp.Business.ServicesImplementations
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EventService(IMapper mapper, IUnitOfWork unitOfWork, IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<EventDto> GetEventByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Events.GetEventByIdAsync(id);

                if (entity == null)
                    throw new ArgumentException("Failed to find record in the database that match the specified id. ", nameof(id));

                var dto = _mapper.Map<EventDto>(entity);
                return dto;
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return null;
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return null;
            }
        }

        public async Task<List<EventDto>> GetEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllEvent().ToListAsync();

            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<(int success, string message)> CreateEventAsync(EventDto dto)
        {
            try
            {
                var isExist = await _unitOfWork.Events.IsEventExistAsync(dto.Name);

                if (isExist)
                {
                    return (0, "The same entry already exists in the storage.");
                }

                var isOwnerExist = await _unitOfWork.Users.IsUserExistsAsync(dto.Name);

                if (isOwnerExist)
                {
                    return (0, "Specified owner doest exist");
                }

                var user = await _userService.GetUserByEmailAsync(dto.Owner);
                dto.UserId = user.Id;
                var entity = _mapper.Map<Event>(dto);

                if (entity == null)
                    throw new ArgumentException("Mapping EventDto to Event was not possible.", nameof(dto));

                await _unitOfWork.Events.AddEventAsync(entity);
                var result = await _unitOfWork.Commit();

                return (result, "Event was created");
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return (0,"One of the fileds was null");
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return (0,"Server promlems");
            }
        }

        public async Task<int> UpdateAsync(Guid id, EventDto dto)
        {
            try
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
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return 0;
            }
            catch (Exception ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return 0;
            }
        }

        public async Task<int> PatchEventAsync(Guid id, EventDto dto)
        {
            var sourceDto = await GetEventByIdAsync(id);

            if (sourceDto == null)
            {
                return 0;
            }

            dto.Id = sourceDto.Id;
            dto.UserId = sourceDto.UserId;

            var patchList = new List<PatchModel>();

            foreach (PropertyInfo property in typeof(EventDto).GetProperties())
            {
                if (!property.GetValue(dto).Equals(property.GetValue(sourceDto)))
                {
                    patchList.Add(new PatchModel()
                    {
                        PropertyName = property.Name,
                        PropertyValue = property.GetValue(dto)
                    });
                }
            }

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
    }
}
