using System.Security.Claims;
using Beam.Application.Filters;
using Beam.Application.Services.Interfaces;
using Beam.Core.Exceptions;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Api.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/post/{postId}/likes")]
    public class PostLikesController : ControllerBase
    {
        private readonly IPostLikeService _postLikeService;
        private readonly IPostLikeNotificationService _postLikeNotificationService;
        private readonly IPostService _postService;
       

        public PostLikesController(IPostLikeService postLikeService, IPostLikeNotificationService postLikeNotificationService, IPostService postService)
        {
            _postLikeService = postLikeService;
            _postLikeNotificationService = postLikeNotificationService;
            _postService = postService;
            
        }
        
        [HttpPost]
        public async Task<IActionResult> ToggleLike(Guid postId)
        {
            var userId = _postLikeService.GetCurrentUserId();  // Получаем Id текущего пользователя

            if (userId == Guid.Empty)
            {
                return Unauthorized();  // Если нет пользователя, возвращаем ошибку авторизации
            }

            try
            {
                await _postLikeService.ToggleLikeAsync(postId, userId);

                // Пересчитываем лайки после обновления
                var likesCount = await _postLikeService.GetLikeCountAsync(postId);
                var isLiked = await _postLikeService.IsPostLikedAsync(postId); // Проверяем текущего пользователя

                var post = await _postService.GetByIdAsync(postId);  // Получаем обновленный пост

                var postDto = new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Content = post.Content,
                    UserName = post.UserName,
                    CreationDate = post.CreationDate,
                    LikesCount = likesCount,  // ✅ Корректно пересчитываем количество лайков
                    IsLiked = isLiked // ✅ Проверяем, лайкал ли текущий пользователь
                };

                return Ok(postDto);  // Возвращаем обновленный PostDto
            }
            catch (Exception ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }
        
        
        // Получение всех лайков с фильтром
        [HttpGet]
        public async Task<IActionResult> GetLikes([FromQuery] PostLikeFilter filter)
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                throw new BadRequestException("User ID is missing from the token.");
            }

            try
            {
                var userId = Guid.Parse(userIdFromToken);
                filter.UserId = userId;

                var likes = await _postLikeService.GetAllAsync(filter);

                return Ok(likes);
            }
            catch (FormatException)
            {
                throw new BadRequestException("Invalid user ID format.");
            }
        }

        // Проверка, поставил ли пользователь лайк на пост
        [HttpGet("/status")]
        public async Task<IActionResult> CheckLikeStatus([FromRoute] Guid postId)
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                throw new BadRequestException("User ID is missing from the token.");
            }

            try
            {
                var userId = Guid.Parse(userIdFromToken);
                var isLiked = await _postLikeService.IsPostLikedAsync(postId);

                return Ok(isLiked);
            }
            catch (FormatException)
            {
                throw new BadRequestException("Invalid user ID format.");
            }
        }
        
      
    }
}
