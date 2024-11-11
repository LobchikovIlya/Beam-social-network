using Beam.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/userroles")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpPost]
    [Route("{userId:guid}/assign/{roleId:int}")]
    public async Task<IActionResult> AssignRoleToUserAsync(Guid userId, int roleId)
    {
        await _userRoleService.AssignRoleToUserAsync(userId, roleId);

        return Ok();
    }
    [HttpDelete("{userId}/remove/{roleId}")]
    public async Task<IActionResult> RemoveRoleToUserAsync(Guid userId, int roleId)
    {
        await _userRoleService.RemoveRoleToUserAsync(userId, roleId);
        
        return NoContent();
    }
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetURolesToUserAsync(Guid userId)
    {
        var roles = await _userRoleService.GetRolesToUserAsync(userId);
        
        return Ok(roles);
    }
}