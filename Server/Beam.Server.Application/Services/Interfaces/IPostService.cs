using Beam.Application.Dto;
using Beam.Infrastructure.Entities;

namespace Beam.Application.Services.Interfaces;

public interface IPostService
{   Task<List<PostDto>> GetAllAsync();
    Task<PostDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(PostInputDto input,Guid userId);
    Task UpdateAsync(Guid id, PostInputDto input);
    Task DeleteByIdAsync(Guid id);
}