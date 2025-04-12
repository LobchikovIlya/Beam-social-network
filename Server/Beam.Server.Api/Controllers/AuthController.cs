
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Beam.UI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IUserRoleService _userRoleService;
    private readonly IHubContext<ChatHub> _hubContext;

    public AuthController(IUserService userService, ITokenService tokenService,IUserRoleService userRoleService, IHubContext<ChatHub> hubContext)
    {
        _userService = userService;
        _tokenService = tokenService;
        _userRoleService = userRoleService;
        _hubContext = hubContext;
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
            if (_hubContext != null)
            {
                Console.WriteLine("📡 Отправка события 'UserListUpdated' в SignalR...");
                await _hubContext.Clients.All.SendAsync("UserListUpdated");
            }
            else
            {
                Console.WriteLine("❌ _hubContext == null, SignalR не работает!");
            }
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
       
        await _userService.SetOnlineStatusAsync(user.Id, true);
        
        var token =await _tokenService.GenerateTokenAsync(user);
        var chatHub = _hubContext.Clients.All;
        await chatHub.SendAsync("UsersStatusChanged", user.Id, true);
        return Ok(new { Token = token });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] Guid userId)
    {
        await _userService.LogoutAsync(userId);
        await _hubContext.Clients.All.SendAsync("UsersStatusChanged", userId, false);
        return Ok(new {message = "Пользователь вышел из системы."});
    }
}
    
