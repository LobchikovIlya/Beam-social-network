using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/comment/{commentId}/likes")]
public class CommentLikesController : ControllerBase
{
    private readonly ICommentLikeService _commentLikeService;

    public CommentLikesController(ICommentLikeService commentLikeService)
    {
        _commentLikeService = commentLikeService;
    }

    [HttpPost]
    
    public async Task<IActionResult> CreateLike([FromRoute]Guid commentId)
    {
        await _commentLikeService.CreateAsync(commentId);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteLike([FromRoute]Guid commentId)
    {
        await _commentLikeService.DeleteByIdAsync(commentId);
        
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetLikes([FromRoute] Guid commentId, [FromQuery] CommentLikeFilter filter)
    {
        var likes = await _commentLikeService.GetAllAsync(filter);
        
        return Ok(likes);
    }
}