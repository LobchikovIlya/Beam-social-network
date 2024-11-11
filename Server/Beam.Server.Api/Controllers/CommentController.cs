using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDto>>> GetAllAsync()
        {
            var comments = await _commentService.GetAllAsync();
            
            return Ok(comments);
        }
       


        [HttpGet("{postId:guid}")]
        public async Task<ActionResult<CommentDto>> GetByIdAsync(Guid postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);

            if (comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }

        [HttpPost("{postId:guid}")]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] CommentInputDto input,Guid postId)
        {
            var commentId = await _commentService.CreateAsync(input,postId);
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
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
}