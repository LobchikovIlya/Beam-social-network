using Beam.Application.Dto;
using Beam.Infrastructure.Entities;

namespace Beam.Application.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetAllAsync();
    
    Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId);
    Task<Guid> CreateAsync(CommentInputDto input,Guid postId);
    Task<Guid> UpdateAsync(Guid id, CommentInputDto input);
    Task DeleteByIdAsync(Guid id);
    
}