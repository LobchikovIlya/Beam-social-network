
using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface IPostService
{   Task<List<PostDto>> GetAllAsync(Guid userId);
    Task<PostDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(PostInputDto input,Guid userId);
    Task UpdateAsync(Guid id, PostInputDto input);
    Task DeleteByIdAsync(Guid id);
    Task <List<PostDto>> GetAllSortedAsync(Guid userId);
}