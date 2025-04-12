using System.ComponentModel.DataAnnotations;
using Beam.Application.Services.Interfaces;
using Beam.Application.Validators;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class PostService : IPostService
{
    private readonly BeamDbContext _dbContext;
    private readonly IUserService _userService;

    public PostService(BeamDbContext dbContext,IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<List<PostDto>> GetAllAsync(Guid userId)
    {
        var posts = await _dbContext.Posts
            .Include(p => p.Likes)
            .Select(p => new PostDto
            {
                Id = p.Id,
                UserId = p.UserId, 
                UserName = p.UserName,
                Content = p.Content,
                CreationDate = p.CreationDate,
                LikesCount = p.Likes.Count, // Подсчитывается на уровне базы данных
                IsLiked = p.Likes.Any(like => like.UserId == userId) // Проверяется на уровне базы данных
            }) 
            .ToListAsync();

        // Преобразуем результат в список DTO
        return posts;

    }


    public async Task<PostDto> GetByIdAsync(Guid id)
    {
        var post = await _dbContext.Posts.FindAsync(id);
        if (post == null)
        {
            throw new NotFoundException($"Post id {id} not found");
        }

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Content = post.Content,
            UserName = post.UserName,
            CreationDate = post.CreationDate
        };
    }

    public async Task<Guid> CreateAsync(PostInputDto input,Guid userId)
    {
        var validator = new PostInputDtoValidator();
        var validationResult = await validator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException();
        }
        var user = await _userService.GetByIdAsync(userId);
        
        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = input.Content,
            UserName = user.Tag,
            CreationDate = DateTimeOffset.UtcNow
        };

        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        return post.Id;
    }

    public async Task UpdateAsync(Guid id, PostInputDto input)
    {
        var post = await _dbContext.Posts.FindAsync(id);
        if (post == null)
        {
            throw new NotFoundException("Post not found");
        }

        post.Content = input.Content;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var post = await _dbContext.Posts.FindAsync(id);
        if (post == null)
        {
            throw new NotFoundException("Post not found");
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<PostDto>> GetAllSortedAsync(Guid userId)
    {
        return await _dbContext.Posts
            .Select(post => new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Content = post.Content,
                CreationDate = post.CreationDate,
                LikesCount = _dbContext.PostLikes.Count(like => like.PostId == post.Id), // ← Подсчет лайков
                IsLiked = _dbContext.PostLikes.Any(like => like.PostId == post.Id && like.UserId == userId) // ← Лайкал ли текущий пользователь
            })
            .OrderByDescending(p => p.CreationDate)
            .ToListAsync();
    }
    
}