﻿using AutoMapper;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.WebAPI.Models.Requests;
using MeetupApp.WebAPI.Models.Responces;
using MeetupApp.WebAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IJwtUtil _jwtUtil;

        public UserController(IUserService userService, IRoleService roleService, IMapper mapper, IJwtUtil jwtUtil)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequestModel request)
        {
            try
            {
                var userRoleId = await _roleService.GetRoleIdForDefaultRoleAsync();
                var userDto = _mapper.Map<UserDto>(request);
                var userWithSameEmailExists = await _userService.IsUserExistsAsync(request.Email);

                if (userDto != null
                    && !userWithSameEmailExists
                    && request.Password.Equals(request.PasswordConfirmation))
                {
                    userDto.RoleId = userRoleId;
                    var result = await _userService.RegisterUserAsync(userDto, request.Password);

                    if (result > 0)
                    {
                        var userInDbDto = await _userService.GetUserByEmailAsync(userDto.Email);

                        var response = await _jwtUtil.GenerateTokenAsync(userInDbDto);

                        return Ok(response);
                    }
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }
    }
}
