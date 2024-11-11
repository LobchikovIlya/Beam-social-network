using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Beam.Application.Services;

public class CommentLikeService : ICommentLikeService
{
    private readonly BeamDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommentLikeService(BeamDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<List<CommentLike>> GetAllAsync(CommentLikeFilter filter)
    {
        var query = _dbContext.CommentLikes.AsQueryable();

        if (filter.CommentId.HasValue)
        {
            query = query.Where(cl => cl.CommentId == filter.CommentId.Value);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(cl => cl.UserId == filter.UserId.Value);
        }

        return await query.ToListAsync();
    }
    
    public async Task CreateAsync(Guid commentId)
    {
        var userId = GetUserIdFromToken();
        
        var existingLike = await _dbContext.CommentLikes
            .FirstOrDefaultAsync(cl => cl.UserId == userId && cl.CommentId == commentId);

        if (existingLike != null)
        {
            throw new BadRequestException("User has already liked this comment.");
        }
        
        var commentLike = new CommentLike
        {
            CommentId = commentId,
            UserId = userId,
            CreationDate = DateTimeOffset.UtcNow
        };

        _dbContext.CommentLikes.Add(commentLike);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteByIdAsync(Guid commentId)
    {
        var userId = GetUserIdFromToken();
        
        var commentLike = await _dbContext.CommentLikes
            .FirstOrDefaultAsync(cl => cl.UserId == userId && cl.CommentId == commentId);

        if (commentLike == null)
        {
            throw new NotFoundException("Comment like not found.");
        }
        
        _dbContext.CommentLikes.Remove(commentLike);
        await _dbContext.SaveChangesAsync();
    }
    
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authorized.");
        }

        return userId;
    }
}
