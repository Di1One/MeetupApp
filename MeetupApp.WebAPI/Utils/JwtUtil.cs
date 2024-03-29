﻿using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.WebAPI.Models.Responces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeetupApp.WebAPI.Utils
{
    public class JwtUtil : IJwtUtil
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        public JwtUtil(IConfiguration configuration,
            IRefreshTokenService refreshTokenService)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<TokenResponseModel> GenerateTokenAsync(UserDto dto)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:JwtSecret"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var nowUtc = DateTime.UtcNow;
            var exp = nowUtc.AddMinutes(double.Parse(_configuration["Token:ExpiryMinutes"]))
                .ToUniversalTime();

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")), //jwt uniq id from spec
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString("D")),
                new Claim(ClaimTypes.Role, dto.RoleName),
            };

            var jwtToken = new JwtSecurityToken(_configuration["Token:Issuer"],
                _configuration["Token:Issuer"],
                claims,
                expires: exp,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshTokenValue = Guid.NewGuid();

            await _refreshTokenService.CreateRefreshTokenAsync(refreshTokenValue, dto.Id);

            return new TokenResponseModel()
            {
                AccessToken = accessToken,
                Role = dto.RoleName,
                TokenExpiration = jwtToken.ValidTo,
                UserId = dto.Id,
                RefreshToken = refreshTokenValue
            };
        }

        public async Task<bool> RemoveRefreshTokenAsync(Guid requestRefreshToken)
        {
            var result = await _refreshTokenService.RemoveRefreshTokenAsync(requestRefreshToken);

            if (result == 0)
            {
                return false;
            }

            return true;
        }
    }
}
