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
    //[Authorize]
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
        /// <response code="400">Request contains null object or invalid object type</response>
        /// <response code="500">Unexpected error on the server side.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            try
            {
                if (id.Equals(default))
                    throw new ArgumentNullException(nameof(id), "A non-empty Id is required.");

                var eventDto = await _eventService.GetEventByIdAsync(id);

                if (eventDto == null)
                {
                    return NotFound();
                }

                var response = _mapper.Map<EventResponceModel>(eventDto);

                if (response == null)
                {
                    throw new ArgumentException("Mapping troubles from dto to model", nameof(response));
                }

                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get all event
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all events for the authorized user</response>
        /// <response code="500">Unexpected error on the server side.</response>
        [Route("GetAll")]
        [HttpGet]
        [ProducesResponseType(typeof(List<EventResponceModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllEvents()
        {
            var eventDto = await _eventService.GetEventsAsync();

            var response = new List<EventResponceModel>();

            foreach(var ev in eventDto)
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
        ///  <response code="200">Returns the event for the authorized user</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        /// <response code="500">Unexpected error on the server side.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEvent([FromBody] AddOrUpdateEventRequestModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "One or more object properties are null.");

                var eventDto = _mapper.Map<EventDto>(model);
                var isExist = await _eventService.IsEventExistAsync(model.Name);

                if (isExist)
                    throw new ArgumentException("The same entry already exists in the storage.", nameof(model));

                var isOwnerExist = await _userService.IsUserExistsAsync(eventDto.Name);

                if (isOwnerExist)
                    throw new ArgumentException("Specified owner doest exist", nameof(model));

                if (eventDto != null)
                {
                    var user = await _userService.GetUserByEmailAsync(eventDto.Owner);
                    eventDto.UserId = user.Id;
                    var result = await _eventService.CreateEventAsync(eventDto);

                    return CreatedAtAction(nameof(CreateEvent), model);
                }

                return BadRequest();
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return Conflict(new ErrorModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] AddOrUpdateEventRequestModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "One or more object properties are null.");

                var dto = _mapper.Map<EventDto>(model);

                var result = await _eventService.UpdateAsync(id, dto);

                if (result == 0)
                    throw new ArgumentNullException(nameof(model), "Nothing was chanched.");

                var response = _mapper.Map<EventResponceModel>(dto);

                if (response == null)
                {
                    throw new ArgumentException("Mapping troubles from dto to model", nameof(response));
                }

                return Ok();
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchEvent(Guid id, [FromBody] AddOrUpdateEventRequestModel model)
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
        /// <response code="400">Request contains null object or invalid object type</response>
        /// <response code="500">Unexpected error on the server side.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                if (id.Equals(default))
                    throw new ArgumentNullException(nameof(id), "A non-empty Id is required.");

                await _eventService.DeleteEventAsync(id);

                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500);
            }
        }
    }
}