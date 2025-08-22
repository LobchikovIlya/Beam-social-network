using System.Security.Claims;
using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/comment/{commentId}/likes")]
public class CommentLikesController : ControllerBase
{
    private readonly ICommentLikeService _commentLikeService;
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentLikesController> _logger;

    public CommentLikesController(ICommentLikeService commentLikeService,
        ICommentService commentService,
        ILogger<CommentLikesController> logger)
    {
        _commentLikeService = commentLikeService;
        _commentService = commentService;
        _logger = logger;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCommentLikes(Guid commentId)
    {
        var userId = GetUserId();
        var likesCount = await _commentLikeService.GetLikeCountAsync(commentId);
        var isLiked = await _commentLikeService.IsCommentLikedAsync(commentId, userId);

        return Ok(new { LikesCount = likesCount, IsLiked = isLiked });
    }

    private Guid GetUserId()
    {
        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdFromToken) || !Guid.TryParse(userIdFromToken, out var userId))
        {
            _logger.LogWarning("Попытка доступа без аутентификации.");
            throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");
        }

        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLike(Guid commentId)
    {
        try
        {
            var userId = GetUserId();

            await _commentLikeService.ToggleLikeAsync(commentId);
            var likesCount = await _commentLikeService.GetLikeCountAsync(commentId);
            var isLiked = await _commentLikeService.IsCommentLikedAsync(commentId, userId);

            var comment = await _commentService.GetByIdAsync(commentId);

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                Author = comment.Author,
                Content = comment.Content,
                PostId = comment.PostId,
                CreationDate = comment.CreationDate,
                LikesCount = likesCount,
                IsLiked = isLiked
            };

            return Ok(commentDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Ошибка авторизации");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка в ToggleLike");
            return BadRequest(new { ErrorMessage = "Произошла ошибка при обработке запроса." });
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetLikes([FromQuery] CommentLikeFilter filter)
    {
        try
        {
            filter.UserId = GetUserId();
            var likes = await _commentLikeService.GetAllAsync(filter);
            return Ok(likes);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Ошибка авторизации");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка в GetLikes");
            return BadRequest(new { ErrorMessage = "Произошла ошибка при обработке запроса." });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> CheckLikeStatus(Guid commentId)
    {
        try
        {
            var userId = GetUserId();
            var isLiked = await _commentLikeService.IsCommentLikedAsync(commentId, userId);
            return Ok(isLiked);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Ошибка авторизации");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка в CheckLikeStatus");
            return BadRequest(new { ErrorMessage = "Произошла ошибка при обработке запроса." });
        }
    }
}