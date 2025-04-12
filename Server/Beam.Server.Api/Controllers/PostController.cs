using System.Security.Claims;
using Beam.Application.Services.Interfaces;
using Beam.Shared.Dto;
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
        private readonly IPostNotificationService _postNotificationService;
        

        public PostController(IPostService postService, IUserService userService, IPostNotificationService postNotificationService)
        {
            _postService = postService;
            _userService = userService;
            _postNotificationService = postNotificationService;
        }
        
       

        // Получение всех постов с информацией о лайках пользователя
        [HttpGet]
        public async Task<ActionResult<List<PostDto>>> GetAllAsync()
        {
            // Извлекаем userId из токена
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                return BadRequest("User ID is missing from the token.");
            }

            if (!Guid.TryParse(userIdFromToken, out Guid userId))
            {
                return BadRequest("Invalid user ID format.");
            }

            // Получаем список постов с количеством лайков и состоянием лайка для текущего пользователя
            var posts = await _postService.GetAllSortedAsync(userId);

            // Загружаем имя пользователя для каждого поста
            var userIds = posts.Select(p => p.UserId).Distinct().ToList();
            var users = await _userService.GetUsersByIdsAsync(userIds);

            // Присваиваем имя пользователя без повторных SQL-запросов
            foreach (var post in posts)
            {
                post.UserName = users.FirstOrDefault(u => u.Id == post.UserId)?.Tag ?? "Unknown";
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
                return BadRequest("Invalid user ID.");
            }
           
            var postId = await _postService.CreateAsync(input, userId);
            var createdPost = await _postService.GetByIdAsync(postId);
            await _postNotificationService.NotifyPostCreated(createdPost);

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
