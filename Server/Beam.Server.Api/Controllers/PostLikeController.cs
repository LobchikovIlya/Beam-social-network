using System.Security.Claims;
using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/post/{postId}/likes")]
public class PostLikesController : ControllerBase
{
    private readonly IPostLikeService _postLikeService;

    public PostLikesController(IPostLikeService postLikeService)
    {
        _postLikeService = postLikeService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLike([FromRoute] Guid postId)
    {
        try
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                throw new BadRequestException("User ID is missing from the token.");
            }

            var userId = Guid.Parse(userIdFromToken);
            await _postLikeService.CreateAsync(postId, userId);

            return Ok();
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (FormatException)
        {
            throw new BadRequestException("Invalid user ID format.");
        }
        catch (InternalServerErrorException ex)
        {
            return StatusCode(500, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteLike([FromRoute] Guid postId)
    {
        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdFromToken == null)
        {
            throw new BadRequestException("User ID is missing from the token.");
        }

        var userId = Guid.Parse(userIdFromToken);
        
        await _postLikeService.DeleteAsync(postId, userId);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetLikes([FromQuery] PostLikeFilter filter)
    {
        try
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                throw new BadRequestException("User ID is missing from the token.");
            }

            var userId = Guid.Parse(userIdFromToken);
            filter.UserId = userId;

            var likes = await _postLikeService.GetAllAsync(filter);

            return Ok(likes);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (FormatException)
        {
            throw new BadRequestException("Invalid user ID format.");
        }
        catch (InternalServerErrorException ex)
        {
            return StatusCode(500, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}