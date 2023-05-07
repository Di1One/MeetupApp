using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.WebAPI.Models.Requests;
using MeetupApp.WebAPI.Models.Responces;
using MeetupApp.WebAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MeetupApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtUtil _jwtUtil;

        public TokenController(IUserService userService,
            IJwtUtil jwtUtil)
        {
            _userService = userService;
            _jwtUtil = jwtUtil;
        }


        /// <summary>
        /// Create new access token for the login data model
        /// </summary>
        /// <param name="request">login model</param>
        /// <returns>An access token for an authorized user.</returns>
        /// <response code="200">Returns the access token for the authorized user</response>
        /// <response code="404">Server cannot find the requested resource</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateJwtToken([FromBody] LoginUserRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound();
            }

            var isPassCorrect = await _userService.CheckUserPasswordAsync(request.Email, request.Password);

            if (!isPassCorrect)
            {
                var message = "Password is incorrect.";
                return BadRequest(new ErrorModel { Message = message });
            }

            var response = await _jwtUtil.GenerateTokenAsync(user);

            return Ok(response);
        }


        /// <summary>
        /// Create new token by refresh token.
        /// </summary>
        /// <param name="request">a refresh token value</param>
        /// <returns>new access token for authorized user</returns>
        /// <response code="200">Returns the access token for the authorized user</response>
        /// <response code="404">Server cannot find the requested resource</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        [Route("Refresh")]
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
            {
                return NotFound();
            }

            var response = await _jwtUtil.GenerateTokenAsync(user);

            await _jwtUtil.RemoveRefreshTokenAsync(request.RefreshToken);

            return Ok(response);
        }

        /// <summary>
        /// Revoke access token by the refresh token value
        /// </summary>
        /// <param name="request">a refresh token value</param>
        /// <returns>The Ok status</returns>
        /// <response code="200">an empty</response>
        /// <response code="404">Server cannot find the requested resource</response>
        /// <response code="400">Request contains null object or invalid object type</response>
        [Route("Revoke")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isTokenRevoked = await _jwtUtil.RemoveRefreshTokenAsync(request.RefreshToken);

            if (!isTokenRevoked)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
