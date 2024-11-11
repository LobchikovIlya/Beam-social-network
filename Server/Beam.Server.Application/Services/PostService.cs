using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
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

    public async Task<List<PostDto>> GetAllAsync()
    {
        var posts = await _dbContext.Posts.ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            UserId = p.UserId,
            Content = p.Content,
            CreationDate = p.CreationDate,
            LikesCount = _dbContext.PostLikes.Count(pl => pl.PostId == p.Id),
        }).ToList();
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
        var user = await _userService.GetByIdAsync(userId);
        
        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = input.Content,
            UserName = user.Name,
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
}