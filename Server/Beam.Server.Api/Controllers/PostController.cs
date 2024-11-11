using System.Security.Claims;
using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public PostController(IPostService postService,IUserService userService)
        {
            _postService = postService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostDto>>> GetAllAsync()
        {
            var posts = await _postService.GetAllAsync();
            foreach (var post in posts)
            {
                var user = await _userService.GetByIdAsync(post.UserId);
                post.UserName = user.Name; // Убедитесь, что UserName заполняется
            }

            return Ok(posts);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PostDto>> GetByIdAsync(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] PostInputDto input)
        {
            
            var userIdSt = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdSt, out Guid userId))
            {
                return BadRequest("Invalid user ID."); // Обработка ошибки при невалидном ID
            }
            var postId = await _postService.CreateAsync(input,  userId);
           // var user = await _userService.GetByIdAsync(userId);
            
            var createdPost = await _postService.GetByIdAsync(postId);

            return Ok(createdPost);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] PostInputDto input)
        {
            await _postService.UpdateAsync(id, input);
            
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteByIdAsync(Guid id)
        {
            await _postService.DeleteByIdAsync(id);
            
            return NoContent();
        }
    }
}