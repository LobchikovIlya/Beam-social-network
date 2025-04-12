
using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface ITokenService
{
   Task<string> GenerateTokenAsync(UserDto user);
}