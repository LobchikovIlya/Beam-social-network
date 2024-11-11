using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.UI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IUserRoleService _userRoleService;

    public AuthController(IUserService userService, ITokenService tokenService,IUserRoleService userRoleService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _userRoleService = userRoleService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto input)
    {
        try
        {
            // Конвертация RegisterDto в UserInputDto
            var userInput = new UserInputDto
            {
                Tag = input.Tag,
                Name = input.Name,
                Password = input.Password
            };

            // Создание пользователя
            var user = await _userService.CreateAsync(userInput);
            await _userRoleService.AssignRoleToUserAsync(user.Id, 2);
        
            // Генерация JWT токена
            var token = await _tokenService.GenerateTokenAsync(user); 
        
            return Ok(new { Token = token });
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


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto input)
    {
        var user = await _userService.ValidateUserAsync(input.Tag, input.Password);
        if (user == null)
        {
            return Unauthorized("Неверный логин или пароль.");
        }

        var token =await _tokenService.GenerateTokenAsync(user);
        return Ok(new { Token = token });
    }
}
    
