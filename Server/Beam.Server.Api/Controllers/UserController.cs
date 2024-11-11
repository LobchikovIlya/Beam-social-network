using Microsoft.AspNetCore.Authorization;
using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[ApiController] 
[Route("api/users")]


public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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
}
