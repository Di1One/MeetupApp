using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MeetupApp.Business.ServicesImplementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateRefreshTokenAsync(Guid tokenValue, Guid userId)
        {
            var rt = new RefreshToken()
            {
                Id = Guid.NewGuid(),
                Token = tokenValue,
                UserId = userId
            };

            await _unitOfWork.RefreshToken.AddTokenAsync(rt);
            var result = await _unitOfWork.Commit();
            return result;
        }

        public async Task<int> RemoveRefreshTokenAsync(Guid tokenValue)
        {
            try
            {
                var token = await _unitOfWork.RefreshToken
                    .GetAllToken()
                    .FirstOrDefaultAsync(token => token.Token.Equals(tokenValue));

                if (token != null)
                {
                    _unitOfWork.RefreshToken.RemoveToken(token);
                }
                else
                {
                    throw new ArgumentException("Could not find a token with the specified model . ", nameof(tokenValue));
                }

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
    }
}
