using Beam.Infrastructure.Entities;


namespace Beam.Application.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    
    Task<User> GetByIdAsync(Guid id);
    
    Task<Guid> CreateAsync(User input);
    
    Task<Guid> UpdateAsync(Guid id, User input);
    
    Task DeleteByIdAsync(Guid id);
}