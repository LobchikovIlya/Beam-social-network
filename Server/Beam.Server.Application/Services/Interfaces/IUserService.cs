using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;


namespace Beam.Application.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    
    Task<User> GetByIdAsync(Guid id);
    
    Task<UserDto> CreateAsync(UserInputDto input);
    
    Task<Guid> UpdateAsync(Guid id, User input);
    
    Task DeleteByIdAsync(Guid id);
}