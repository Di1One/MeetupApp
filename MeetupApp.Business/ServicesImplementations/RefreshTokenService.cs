using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.Business.ServicesImplementations
{
    public class RefreshTokenService
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

            await _unitOfWork.RefreshToken.AddAsync(rt);
            var result = await _unitOfWork.Commit();
            return result;
        }

        public async Task<int> RemoveRefreshTokenAsync(Guid tokenValue)
        {
            var token = await _unitOfWork.RefreshToken
                .Get()
                .FirstOrDefaultAsync(token => token.Token.Equals(tokenValue));

            if (token != null)
                _unitOfWork.RefreshToken.Remove(token);

            return await _unitOfWork.Commit();
        }
    }
}
