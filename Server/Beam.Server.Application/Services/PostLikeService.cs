using System.Security.Claims;
using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class PostLikeService : IPostLikeService
{
    private readonly BeamDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<ChatHub> _hubContext;

    public PostLikeService(BeamDbContext dbContext, IHttpContextAccessor httpContextAccessor,
        IHubContext<ChatHub> hubContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _hubContext = hubContext;
    }

    public async Task ToggleLikeAsync(Guid postId, Guid userId)
    {
        // Ищем существующий лайк
        var existingLike = await _dbContext.PostLikes
            .FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);

        if (existingLike != null)
        {
            // Если лайк существует, удаляем его
            _dbContext.PostLikes.Remove(existingLike);
        }
        else
        {
            // Если лайка нет, добавляем новый
            var newLike = new PostLike
            {
                UserId = userId,
                PostId = postId,
                CreationDate = DateTimeOffset.UtcNow
            };
            _dbContext.PostLikes.Add(newLike);
        }

        // Сохраняем изменения
        await _dbContext.SaveChangesAsync();
        var likesCount = await GetLikeCountAsync(postId);
        await _hubContext.Clients.All.SendAsync("ReceivePostLiked", new PostDto
        {
            Id = postId,
            LikesCount = likesCount
        });
        //var isLiked = await _dbContext.PostLikes
        // .AnyAsync(like => like.UserId == userId && like.PostId == postId);

        // await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceivePostLikedStatus", postId, isLiked);
    }


    public async Task UpdateLikesCountAsync(Guid postId, int likesCount)
    {
        var postDto = new PostDto
        {
            Id = postId,
            LikesCount = likesCount
        };

        await _hubContext.Clients.All.SendAsync("UpdateLikesCount", postDto);
    }

    public async Task<int> GetLikeCountAsync(Guid postId)
    {
        return await _dbContext.PostLikes.CountAsync(pl => pl.PostId == postId);
    }

    public Guid GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) throw new UnauthorizedAccessException("HTTP контекст недоступен.");

        var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
            throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");

        return Guid.Parse(userIdString);
    }


    public async Task<List<PostLike>> GetAllAsync(PostLikeFilter filter)
    {
        var query = _dbContext.PostLikes.AsQueryable();

        if (filter.PostId.HasValue)
            query = query.Where(pl => pl.PostId == filter.PostId.Value);

        if (filter.UserId.HasValue)
            query = query.Where(pl => pl.UserId == filter.UserId.Value);

        return await query.ToListAsync();
    }

    public async Task<PostDto> CreateAsync(Guid postId)
    {
        var userId = GetCurrentUserId();

        // Включаем или отключаем лайк
        await ToggleLikeAsync(postId, userId);

        // Обновляем количество лайков
        var likesCount = await GetLikeCountAsync(postId);

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId)
                   ?? throw new NotFoundException("Пост не найден.");

        var postDto = new PostDto
        {
            Id = post.Id,
            UserId = userId,
            Content = post.Content,
            LikesCount = likesCount,
            IsLiked = await IsPostLikedAsync(postId) // Проверка, поставил ли пользователь лайк
        };

        // Отправляем обновленную информацию всем клиентам
        await _hubContext.Clients.Client(userId.ToString()).SendAsync("ReceivePostLiked", postDto);
        await _hubContext.Clients.All.SendAsync("UpdateLikesCount", new { postId, likesCount });

        return postDto;
    }

    public async Task<bool> IsPostLikedAsync(Guid postId)
    {
        var userId = GetCurrentUserId();
        return await _dbContext.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
    }

    public async Task<PostDto> DeleteAsync(Guid postId)
    {
        var userId = GetCurrentUserId();

        // Включаем или отключаем лайк
        await ToggleLikeAsync(postId, userId);

        // Обновляем количество лайков
        var likesCount = await GetLikeCountAsync(postId);

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId)
                   ?? throw new NotFoundException("Пост не найден.");

        var postDto = new PostDto
        {
            Id = post.Id,
            UserId = userId,
            Content = post.Content,
            LikesCount = likesCount,
            IsLiked = await IsPostLikedAsync(postId) // Проверка, поставил ли пользователь лайк
        };

        // Отправляем обновленную информацию всем клиентам
        await _hubContext.Clients.Client(userId.ToString()).SendAsync("ReceivePostUnLiked", postDto);
        await _hubContext.Clients.All.SendAsync("UpdateLikesCount", new { postId, likesCount });

        return postDto;
    }
}