using Beam.Application.Filters;
using Beam.Infrastructure.Entities;

namespace Beam.Application.Services.Interfaces;

public interface ICommentLikeService
{
    Task<List<CommentLike>> GetAllAsync(CommentLikeFilter filter);

    Task CreateAsync(Guid commentId);

    Task DeleteByIdAsync(Guid commentId);
}