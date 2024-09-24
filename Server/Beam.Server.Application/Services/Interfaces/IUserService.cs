
using Beam.Application.Dto;


namespace Beam.Application.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    
    Task<UserDto> GetByIdAsync(Guid id);
    
    Task<UserDto> CreateAsync(UserInputDto input);
    
    Task<Guid> UpdateAsync(Guid id, UserInputDto input);
    
    Task DeleteByIdAsync(Guid id);
}