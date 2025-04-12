using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Shared.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[ApiController] 
[Route("api/users")]


public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private  readonly IUserActivityService _userActivityService;

    public UserController(IUserService userService, IUserActivityService userActivityService)
    {
        _userService = userService;
        _userActivityService = userActivityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();
        
        return Ok(users);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute]Guid id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Произошла непредвиденная ошибка.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UserInputDto input)
    {
        try
        {
            var user = await _userService.CreateAsync(input);
            var createUser = await _userService.GetByIdAsync(user.Id);
            return Ok(createUser);
        }
      
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Произошла непредвиденная ошибка.");
        }
    }
    [Authorize(Roles = "User")]
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute]Guid id, [FromBody] UserInputDto input)
    {
        try
        {
            await _userService.UpdateAsync(id, input);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Произошла непредвиденная ошибка.");
        }
    }
    [Authorize(Roles = "User")]
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]Guid id)
    {
        try
        {
            await _userService.DeleteByIdAsync(id);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Произошла непредвиденная ошибка.");
        }
    }
    
    [HttpPost("update-activity")]
    public async Task<IActionResult> UpdateActivity()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "id");
        if (userIdClaim == null)
        {
            return BadRequest("Не найден claim с userId в токене");
        }
        
        var userId = Guid.Parse(userIdClaim.Value);
        UserActivityDto userActivityDto = new UserActivityDto();
        userActivityDto.UserId = userId;
        userActivityDto.LastActivityTime = DateTimeOffset.UtcNow;
        
        await _userActivityService.UpdateActivityAsync(userActivityDto.UserId,userActivityDto.LastActivityTime);
        return Ok();
    }
}
