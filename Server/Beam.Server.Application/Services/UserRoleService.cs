using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly BeamDbContext _dbContext;

    public UserRoleService(BeamDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AssignRoleToUserAsync(Guid userId, int roleId)
    {
        var userToRole = new UsersToRole { UserId = userId, RoleId = roleId };
        await _dbContext.UsersToRoles.AddAsync(userToRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveRoleToUserAsync(Guid userId, int roleId)
    {
        var userToRole =
            await _dbContext.UsersToRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userToRole == null) throw new NotFoundException("User with this role not found");
        _dbContext.UsersToRoles.Remove(userToRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Role>> GetRolesToUserAsync(Guid userId)
    {
        return await _dbContext.UsersToRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .ToListAsync();
    }
}