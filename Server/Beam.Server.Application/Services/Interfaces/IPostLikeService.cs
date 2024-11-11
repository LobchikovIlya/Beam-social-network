using Beam.Infrastructure.Entities;

namespace Beam.Application.Services.Interfaces;

public interface IPostLike
{
    Task<List<PostLike>> GetAllAsync();

    Task<PostLike> GetByIdAsync(Guid userId, Guid postId);

    Task<Guid> CreateAsync(PostLike input);

    Task<Guid> UpdateAsync(Guid id, PostLike input);

    Task DeleteByIdAsync(Guid id);
}