using Beam.Application.Filters;
using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface ICommentLikeService
{
    Task<List<CommentLike>> GetAllAsync(CommentLikeFilter filter);
    Task ToggleLikeAsync(Guid commentId);
    Task<CommentDto> CreateAsync(Guid commentId);
    Task<int> GetLikeCountAsync(Guid commentId);
    Task<CommentDto> DeleteByIdAsync(Guid commentId);
    Guid GetCurrentUserId();
    Task<bool> IsCommentLikedAsync(Guid commentId,Guid userId);
    Task<List<CommentDto>> GetCommentsWithLikesAsync(Guid userId);
    Task UpdateCommentLikesCountAsync(Guid commentId, int likesCount);
   
}