using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetAllAsync();
    Task<CommentDto> GetByIdAsync(Guid id);
    Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId, Guid? userId = null);
    Task<Guid> CreateAsync(CommentInputDto input, Guid postId);
    Task<Guid> UpdateAsync(Guid id, CommentInputDto input);
    Task DeleteByIdAsync(Guid id);
}