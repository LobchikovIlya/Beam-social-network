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
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ITokenService _tokenService;
    private readonly IUserRoleService _userRoleService;
    private readonly IUserService _userService;
    private readonly IUserAuthService _userAuthService;
    private readonly IUserActivityService _userActivityService;

    public AuthController(IUserService userService,IUserAuthService userAuthService, ITokenService tokenService, IUserRoleService userRoleService,IUserActivityService userActivityService,
        IHubContext<ChatHub> hubContext)
    {
        _userService = userService;
        _userAuthService = userAuthService;
        _userActivityService = userActivityService;
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
            var response = new RegisterResponseDto
            {
                Token = token,
                UserId = user.Id,
                UserName = user.Tag,
            };

            return Ok(response);
        }
        catch (FluentValidation.ValidationException ex)
        {
          var errors = ex.Errors
              .GroupBy(e => e.PropertyName)
              .ToDictionary(
                  g => g.Key,
                  g => g.Select(e => e.ErrorMessage).ToArray()
                  );
            return BadRequest(new{errors});
        }
        catch (BadRequestException ex)
        {
            // например, ник уже существует
            var errors = new Dictionary<string, string[]>
            {
                { "Tag", new[] { ex.Message } }
            };
            return BadRequest(new { errors });
        }
        catch (Exception ex)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "", new[] { "Произошла непредвиденая ошибка ." } },
            };
            return StatusCode(500,new{errors} );
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDto input)
    {
        var user = await _userAuthService.ValidateUserAsync(input.Tag, input.Password);
        if (user == null) return Unauthorized("Неверный логин или пароль.");

        await _userActivityService.SetOnlineStatusAsync(user.Id, true);

        var token = await _tokenService.GenerateTokenAsync(user);
        var chatHub = _hubContext.Clients.All;
        await chatHub.SendAsync("UsersStatusChanged", user.Id, true);
        var response = new LoginResponseDto
        {
            Token = token,
            UserName = user.Name,
            UserId = user.Id
        };
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] Guid userId)
    {
        await _userAuthService.LogoutAsync(userId);
        await _hubContext.Clients.All.SendAsync("UsersStatusChanged", userId, false);
        return Ok(new { message = "Пользователь вышел из системы." });
    }
}