using System.ComponentModel.DataAnnotations;
using Beam.Application.Services.Interfaces;
using Beam.Application.Validators;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class CommentService : ICommentService
{
    private readonly BeamDbContext _dbContext;
    private readonly ICommentNotificationService _сommentNotificationService;

    public CommentService(BeamDbContext dbContext,ICommentNotificationService сommentNotificationService)
    {
        _dbContext = dbContext;
        _сommentNotificationService = сommentNotificationService;
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
   
    public async Task<CommentDto> GetByIdAsync(Guid id)
    {
        var comment = await _dbContext.Comments
            .Where(c => c.Id == id)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                Author = c.Author,
                Content = c.Content,
                CreationDate = c.CreationDate,
                LikesCount = c.CommentLikes.Count, // Подсчёт лайков
            })
            .FirstOrDefaultAsync();

        if (comment == null)
        {
            throw new NotFoundException("Comment not found");
        }

        return comment;
    }

    public async Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId, Guid? userId = null)
    {
        var commentsQuery = _dbContext.Comments
            .Where(c => c.PostId == postId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                Author = c.Author,
                Content = c.Content,
                CreationDate = c.CreationDate,
                LikesCount = c.CommentLikes.Count(), // Подсчёт лайков
                IsLiked = userId.HasValue && c.CommentLikes.Any(cl => cl.UserId == userId.Value) // Проверка лайка
            });

        return await commentsQuery.ToListAsync();
    }


    

    public async Task<Guid> CreateAsync(CommentInputDto input, Guid postId)
    {
        var validator = new CommentInputDtoValidator();
        var validationResult = await validator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException();
        }
       
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

        var commentDto = new CommentDto
        {
            Id = newComment.Id,
            PostId = newComment.PostId,
            Author = newComment.Author,
            Content = newComment.Content,
            CreationDate = newComment.CreationDate,
            LikesCount = newComment.CommentLikes.Count,

        };
        await _сommentNotificationService.NotifyCommentCreated(commentDto);
        
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
        var updatedComment = new CommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Author = comment.Author,
            Content = comment.Content,
            CreationDate = comment.CreationDate,
            LikesCount = comment.CommentLikes.Count
        };
        await _сommentNotificationService.NotifyCommentUpdated(updatedComment);


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