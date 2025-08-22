using Beam.Shared.Dto;

namespace Beam.UI.Interfaces;

public interface IUiUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
}