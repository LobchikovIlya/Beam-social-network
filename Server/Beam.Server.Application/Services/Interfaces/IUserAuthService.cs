namespace Beam.Application.Services.Interfaces;
using Beam.Shared.Dto;

public interface IUserAuthService
{
    Task<UserDto> ValidateUserAsync(string username, string password);
   
    Task LogoutAsync(Guid userId);
}