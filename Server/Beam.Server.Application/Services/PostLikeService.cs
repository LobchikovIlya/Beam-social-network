using System.Security.Claims;
using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Beam.Application.Services;

public class PostLikeService : IPostLikeService
{
    private readonly BeamDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostLikeService(BeamDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return Guid.Parse(userIdString);
    }

    public async Task<List<PostLike>> GetAllAsync(PostLikeFilter filter)
    {
        var query = _dbContext.PostLikes.AsQueryable();
        if (filter.PostId.HasValue)
        {
            query = query.Where(pl => pl.PostId == filter.PostId.Value);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(pl => pl.UserId == filter.UserId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task CreateAsync(Guid postId, Guid userId)
    {
        var currentUserId = GetCurrentUserId();

        var existingLike = await _dbContext.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == currentUserId);

        if (existingLike != null)
        {
            throw new BadRequestException("User has already liked this post.");
        }

        var postLike = new PostLike()
        {
            PostId = postId,
            UserId = currentUserId,
            CreationDate = DateTimeOffset.UtcNow
        };

        _dbContext.PostLikes.Add(postLike);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task ToggleLikeAsync(Guid postId)
    {
        var currentUserId = GetCurrentUserId();

        // Проверяем, существует ли уже лайк этого пользователя на пост
        var existingLike = await _dbContext.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == currentUserId);

        if (existingLike != null)
        {
            _dbContext.PostLikes.Remove(existingLike);
        }
        else
        {
            var postLike = new PostLike()
            {
                PostId = postId,
                UserId = currentUserId,
                CreationDate = DateTimeOffset.UtcNow
            };

            _dbContext.PostLikes.Add(postLike);
        }

        await _dbContext.SaveChangesAsync();
    }


    public async Task DeleteAsync(Guid postId, Guid userId)
    {
        var currentUserId = GetCurrentUserId();
        var postLike = await _dbContext.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == currentUserId);

        if (postLike == null)
        {
            throw new NotFoundException("Post like not found.");
        }

        _dbContext.PostLikes.Remove(postLike);
        await _dbContext.SaveChangesAsync();
    }
}