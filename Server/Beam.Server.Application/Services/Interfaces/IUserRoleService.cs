using Beam.Infrastructure.Entities;

namespace Beam.Application.Services.Interfaces;

public interface IUserRoleService
{
    Task AssignRoleToUserAsync(Guid userId, int roleId);
    Task RemoveRoleToUserAsync(Guid UserId, int roleId);
    Task<List<Role>> GetRolesToUserAsync(Guid userId);
}