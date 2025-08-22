using System.Security.Claims;
using Beam.Application.Services.Interfaces;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly ICommentNotificationService _commentNotificationService;
    private readonly ICommentService _commentService;
    private readonly IUserService _userService;

    public CommentController(ICommentService commentService, IUserService userService,
        ICommentNotificationService commentNotificationService)
    {
        _commentService = commentService;
        _userService = userService;
        _commentNotificationService = commentNotificationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentDto>>> GetAllAsync()
    {
        var comments = await _commentService.GetAllAsync();

        return Ok(comments);
    }


    [HttpGet("{postId:guid}")]
    public async Task<ActionResult<List<CommentDto>>> GetCommentsByPostIdAsync(Guid postId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var userGuid = Guid.Parse(userId);

        var comments = await _commentService.GetCommentsByPostIdAsync(postId, userGuid);

        return Ok(comments);
    }

    [HttpPost("{postId:guid}")]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CommentInputDto input, Guid postId)
    {
        // Извлечение UserId из токена
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Преобразование в Guid
        input.UserId = Guid.Parse(userId);
        // Создание комментария
        var commentId = await _commentService.CreateAsync(input, postId);
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);
        var comment = await _commentService.GetByIdAsync(commentId);
        await _commentNotificationService.NotifyCommentCreated(comment);
        return Ok(comments);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CommentInputDto input)
    {
        await _commentService.UpdateAsync(id, input);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteByIdAsync(Guid id)
    {
        await _commentService.DeleteByIdAsync(id);
        return NoContent();
    }
}