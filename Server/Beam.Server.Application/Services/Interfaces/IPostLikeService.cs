using Beam.Application.Filters;
using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface IPostLikeService
{
    Task<int> GetLikeCountAsync(Guid postId);
    Task<List<PostLike>> GetAllAsync(PostLikeFilter filter);
    Task ToggleLikeAsync(Guid postId, Guid userId);

    Task<PostDto> CreateAsync(Guid postId);

    Task UpdateLikesCountAsync(Guid postId, int likesCount);

    Task<bool> IsPostLikedAsync(Guid postId);
    Guid GetCurrentUserId();
    Task<PostDto> DeleteAsync(Guid postId);
}