using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Application.Services;

public class CommentLikeService : ICommentLikeService
{
    private readonly BeamDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<ChatHub> _hubContext;
    public CommentLikeService(BeamDbContext dbContext, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> hubContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _hubContext = hubContext;
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
    
    public async Task<CommentDto> CreateAsync(Guid commentId)
    {
        var userId = GetCurrentUserId();
        await ToggleLikeAsync(commentId);
        var likesCount = await GetLikeCountAsync(commentId);
        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId)
                   ?? throw new NotFoundException("Комент не найден.");
        
        var commentDto = new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            LikesCount = likesCount,
            IsLiked = await IsCommentLikedAsync(commentId,userId) // Проверка, поставил ли пользователь лайк
        };
        await _hubContext.Clients.Client(userId.ToString()).SendAsync("ReceiveCommentLikeStatus", commentDto);
        
        await _hubContext.Clients.AllExcept(userId.ToString()).SendAsync("ReceiveCommentLikeStatus", new CommentDto
        {
         Id = commentId, 
         LikesCount = likesCount,
         
        });
       /* await _hubContext.Clients.AllExcept(userId.ToString()).SendAsync("ReceiveCommentLikeStatus", new CommentDto
        {
            Id = comment.Id,
            LikesCount = likesCount,
            
        });*/
        
        return commentDto;
    }
    
   

    
    public async Task<int> GetLikeCountAsync(Guid commentId)
    {
        return await _dbContext.CommentLikes.CountAsync(pl => pl.CommentId == commentId);
    }


    public async Task<CommentDto> DeleteByIdAsync(Guid commentId)
    {
        var userId = GetCurrentUserId();

        // Включаем или отключаем лайк
        await ToggleLikeAsync(commentId);

        // Обновляем количество лайков
        var likesCount = await GetLikeCountAsync(commentId);

        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId)
                   ?? throw new NotFoundException("Пост не найден.");
       
        var commentDto = new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            LikesCount = likesCount,
            IsLiked = await IsCommentLikedAsync(commentId, userId)
        };
        await _hubContext.Clients.Client(userId.ToString()).SendAsync("ReceiveCommentUnLiked", commentDto);
        await _hubContext.Clients.All.SendAsync("ReceiveCommentLikeStatus", new CommentDto
        {
            Id = commentId,
            LikesCount = likesCount,
           
          
            
        });

        return commentDto;
    }
    
    
    public Guid GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("HTTP контекст недоступен.");
        }

        var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");
        }

        return Guid.Parse(userIdString);
    }
    public async Task<List<CommentDto>> GetCommentsWithLikesAsync(Guid userId)
    {
        var comments = await _dbContext.Comments
            .Select(comment => new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                LikesCount = _dbContext.CommentLikes.Count(cl => cl.CommentId == comment.Id),
                IsLiked = _dbContext.CommentLikes.Any(cl => cl.UserId == userId && cl.CommentId == comment.Id)
            })
            .ToListAsync();

        return comments;
    }
    public async Task ToggleLikeAsync(Guid commentId)
    {
        var userId = GetCurrentUserId();
        // Ищем существующий лайк
        var existingLike = await _dbContext.CommentLikes
            .FirstOrDefaultAsync(like => like.UserId == userId && like.CommentId == commentId);

        if (existingLike != null)
        {
            // Если лайк существует, удаляем его
            _dbContext.CommentLikes.Remove(existingLike);
        }
        else
        {
            // Если лайка нет, добавляем новый
            var newLike = new CommentLike
            {
                UserId = userId,
                CommentId = commentId,
                CreationDate = DateTimeOffset.UtcNow,
                
            };
            _dbContext.CommentLikes.Add(newLike);
            
        }

        // Сохраняем изменения
        await _dbContext.SaveChangesAsync();
        var likesCount = await GetLikeCountAsync(commentId);
      
        await _hubContext.Clients.All.SendAsync("ReceiveCommentLikedStatus", new CommentDto
        {
            Id = commentId,
            LikesCount = likesCount,
           
        });
        
    }
    
    public async Task<bool> IsCommentLikedAsync(Guid commentId,Guid userId)
    {
      
        return await _dbContext.CommentLikes.AnyAsync(pl => pl.CommentId == commentId && pl.UserId == userId);
    }
    public async Task UpdateCommentLikesCountAsync(Guid commentId, int likesCount)
    {
        var commentDto = new CommentDto
        {
            Id = commentId,
            LikesCount = likesCount
        };

        await _hubContext.Clients.All.SendAsync("UpdateLikesCount", commentDto);
    }



}
