using Beam.Application.Filters;
using Beam.Infrastructure.Entities;

namespace Beam.Application.Services.Interfaces;

public interface IPostLikeService
{
    Task<List<PostLike>> GetAllAsync(PostLikeFilter filter);
    
    Task CreateAsync(Guid postId, Guid userId);

    Task DeleteAsync(Guid postId, Guid userId);
    Task ToggleLikeAsync(Guid postId);
}