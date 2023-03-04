using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MeetupApp.Business.ServicesImplementations
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public RoleService(IUnitOfWork unitOfWork, IConfiguration configuration = null)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Guid> GetRoleIdForDefaultRoleAsync()
        {
            var roleNameByDefault = GetDefaultRoleNameForUser();
            var role = await _unitOfWork.Roles
                .FindBy(r =>
                    r.Name.Equals(roleNameByDefault))
                .AsNoTracking().FirstOrDefaultAsync();
            if (role == null)
                throw new ArgumentException(
                    $"There is no entry in the database matching the default role value: {nameof(roleNameByDefault)}");

            return role.Id;
        }
        private string GetDefaultRoleNameForUser()
        {
            var roleName = _configuration["RoleByDefault"];
            if (roleName == null)
                throw new JsonException(
                    "Failed to retrieve a valid default role value.");

            return roleName;
        }
    }
}
