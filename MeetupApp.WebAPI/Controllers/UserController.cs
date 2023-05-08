﻿using AutoMapper;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.WebAPI.Models.Requests;
using MeetupApp.WebAPI.Models.Responces;
using MeetupApp.WebAPI.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MeetupApp.WebAPI.Controllers
{
    /// <summary>
    /// Controller that provides API endpoints for the User resource.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IJwtUtil _jwtUtil;

        public UserController(IUserService userService, IMapper mapper, IJwtUtil jwtUtil)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Returns the user</response>
        /// <response code="409">Request could not be processed because of conflict in the request</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = _mapper.Map<UserDto>(request);

            var (success, message) = await _userService.RegisterUserAsync(userDto, request.Password);

            if (success)
            {
                var userInDbDto = await _userService.GetUserByEmailAsync(userDto.Email);

                var response = await _jwtUtil.GenerateTokenAsync(userInDbDto);

                return Ok(response);
            } 

            return Conflict(new ErrorModel { Message = message });
        }
    }
}
