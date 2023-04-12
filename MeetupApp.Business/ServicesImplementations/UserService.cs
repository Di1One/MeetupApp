using AutoMapper;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MeetupApp.Business.ServicesImplementations
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleService _roleService;

        public UserService(IMapper mapper, IConfiguration configuration, IUnitOfWork unitOfWork, IRoleService roleService)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _roleService = roleService;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);    

            if (user != null) return _mapper.Map<UserDto>(user);

            throw new ArgumentException("User with specified email doesn't exist. ", nameof(email));
        }

        public async Task<UserDto?> GetUserByRefreshTokenAsync(Guid refreshToken)
        {
            var token = await _unitOfWork.RefreshToken
                .GetAllToken()
                .Include(token => token.User)
                .ThenInclude(user => user.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.Token.Equals(refreshToken));

            if (token != null) return _mapper.Map<UserDto>(token.User);

            throw new ArgumentException("Could not find a token with the specified model . ", nameof(refreshToken));
        }

        public async Task<bool> IsUserExistsAsync(Guid userId)
        {
            return await _unitOfWork.Users.Get().AnyAsync(user => user.Id.Equals(userId));
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _unitOfWork.Users.Get().AnyAsync(user => user.Email.Equals(email));
        }

        public async Task<bool> CheckUserPasswordAsync(string email, string password)
        {
            var dbPasswordHash = (await _unitOfWork.Users
                .Get()
                .FirstOrDefaultAsync(user => user.Email.Equals(email)))
                ?.PasswordHash;

            return dbPasswordHash != null && CreateMd5($"{password}.{_configuration["Secret:PasswordSalt"]}").Equals(dbPasswordHash);
        }

        public async Task<bool> CheckUserPasswordAsync(Guid userId, string password)
        {
            var dbPasswordHash = (await _unitOfWork.Users.GetByIdAsync(userId))?.PasswordHash;

            return dbPasswordHash != null && CreateMd5($"{password}.{_configuration["Secret:PasswordSalt"]}").Equals(dbPasswordHash);
        }

        public async Task<int> RegisterUserAsync(UserDto dto, string password)
        {
            var userRoleId = await _roleService.GetRoleIdForDefaultRoleAsync();
            dto.RoleId = userRoleId;

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = CreateMd5($"{password}.{_configuration["Secret:PasswordSalt"]}");

            await _unitOfWork.Users.AddAsync(user);
            return await _unitOfWork.Commit();
        }

        private string CreateMd5(string password)
        {
            var passwordSalt = _configuration["UserSecrets:PasswordSalt"];

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputByte = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
                var hashByte = md5.ComputeHash(inputByte);

                return Convert.ToHexString(hashByte);
            }
        }
    }
}
