using AutoMapper;
using MeetupApp.Core;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.WebAPI.Models.Requests;
using MeetupApp.WebAPI.Models.Responces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Reflection;

namespace MeetupApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        public EventController(IMapper mapper, IEventService eventService, IUserService userService)
        {
            _mapper = mapper;
            _eventService = eventService;
            _userService = userService;
        }

        /// <summary>
        /// Get the event by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns the event for the authorized user</response>
        /// <response code="404">Server cannot find the requested resource</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventResponceModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            if (ModelState.IsValid)
            {
                var eventDto = await _eventService.GetEventByIdAsync(id);

                if (eventDto == null)
                {
                    return NotFound();
                }

                var response = _mapper.Map<EventResponceModel>(eventDto);

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all event
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all events for the authorized user</response>
        [Route("GetAll")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EventResponceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEvents()
        {
            var eventsDto = await _eventService.GetEventsAsync();

            var response = new List<EventResponceModel>();

            foreach(var ev in eventsDto)
            {
                response.Add(_mapper.Map<EventResponceModel>(ev));
            }

            return Ok(response);
        }

        /// <summary>
        /// Create new event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Returns the event for the authorized user</response>
        /// <response code="409">Request could not be processed because of conflict in the request</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        /// <response code="500">Unexpected error on the server side.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEvent([FromBody] AddorUpdateEventRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var eventDto = _mapper.Map<EventDto>(model);
                var isExist = await _eventService.IsEventExistAsync(model.Name);

                if (isExist)
                {
                    return Conflict("The same entry already exists in the storage.");
                }

                var isOwnerExist = await _userService.IsUserExistsAsync(eventDto.Name);

                if (isOwnerExist)
                {
                    return Conflict("Specified owner doest exist");
                }

                var user = await _userService.GetUserByEmailAsync(eventDto.Owner);
                eventDto.UserId = user.Id;

                var result = await _eventService.CreateEventAsync(eventDto);

                if (result == 0)
                {
                    return StatusCode(500);
                }

                var response = _mapper.Map<EventResponceModel>(eventDto);

                return CreatedAtAction(nameof(CreateEvent), response);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update event 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Returns updated event for the authorized user</response>
        /// <response code="204">Nothing was changed</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        /// <exception cref="ArgumentException"></exception>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EventResponceModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] AddorUpdateEventRequestModel model)
        {
            if (ModelState.IsValid) 
            {
                var dto = _mapper.Map<EventDto>(model);

                var result = await _eventService.UpdateAsync(id, dto);

                if (result == 0)
                {
                    return NoContent();
                }

                var response = _mapper.Map<EventResponceModel>(dto);

                return Ok(response);
            }
            else 
            {
                return BadRequest(); 
            }
        }

        /// <summary>
        /// Edit fields of the event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <response code="200">Returns updated event for the authorized user</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        /// <response code="500">Unexpected error on the server side.</response>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchEvent(Guid id, [FromBody] AddorUpdateEventRequestModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sourceDto = await _eventService.GetEventByIdAsync(id);

                    var dto = _mapper.Map<EventDto>(model);
                    dto.UserId = sourceDto.UserId;
                    dto.Id = id;

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

                    if (patchList.Any())
                    {
                        await _eventService.PatchEventAsync(id, patchList);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status304NotModified, model);
                    }

                    var updatedEvent = await _eventService.GetEventByIdAsync(id);

                    var responseModel = _mapper.Map<EventResponceModel>(updatedEvent);

                    return Ok(responseModel);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Delete the event by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Event was deleted</response>
        /// <response code="400">Request contains null object or invalid object type.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var result = await _eventService.DeleteEventAsync(id);

            if (result != 0)
            {
                return NoContent();
            }

            return BadRequest();
        }
    }
}