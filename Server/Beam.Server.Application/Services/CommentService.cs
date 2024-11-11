using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class CommentService : ICommentService
{
    private readonly BeamDbContext _dbContext;

    public CommentService(BeamDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CommentDto>> GetAllAsync()
    {
        var comments = await _dbContext.Comments.ToListAsync();

        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            PostId = c.PostId,
            Content = c.Content,
            CreationDate = c.CreationDate
        }).ToList();
    }
   

    public async Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId)
    {
        return await _dbContext.Comments
            .Where(c => c.PostId == postId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                Author = c.Author,
                Content = c.Content,
                CreationDate = c.CreationDate
            })
            .ToListAsync();
    }
    

    public async Task<Guid> CreateAsync(CommentInputDto input, Guid postId)
    {
       
        var newComment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            Content = input.Content,
            Author = input.Author,
            CreationDate = DateTimeOffset.UtcNow 
           
        };

        await _dbContext.Comments.AddAsync(newComment);
        await _dbContext.SaveChangesAsync();
        
        return newComment.Id;
    }

    public async Task<Guid> UpdateAsync(Guid id, CommentInputDto input)
    {
        var comment = await _dbContext.Comments.FindAsync(id); 
        if (comment == null)
        {
            throw new NotFoundException("Comment not found");
        }
        
        comment.Content = input.Content;
        
        await _dbContext.SaveChangesAsync();

        return comment.Id;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment == null)
        {
            throw new NotFoundException($"Comment id {id} not found");
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}