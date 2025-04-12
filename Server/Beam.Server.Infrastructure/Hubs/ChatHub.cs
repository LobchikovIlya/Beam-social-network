using System.Collections.Concurrent;
using System.Security.Claims;
using Beam.Infrastructure.Entities;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace Beam.Infrastructure.Hubs;
//[Authorize]
public class ChatHub : Hub
{
    
    private static readonly ConcurrentDictionary<string, Guid> OnlineUsers = new();
// Методы для работы с комментариями
    public async Task JoinGroup(string postId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, postId);
    }

    public async Task LeaveGroup(string postId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId);
    }

    public async Task SendCommentToGroup(Guid postId, string commentContent)
    {
        var commentDto = new CommentDto
        {
            Content = commentContent,
            PostId = postId,
            CreationDate = DateTimeOffset.UtcNow
        };

        await Clients.Group(postId.ToString()).SendAsync("ReceiveCommentCreated", commentDto);
    }


    // Методы для работы с постами
    public async Task JoinPostGroup(Guid postId)
    {
        // Присоединяем пользователя к группе по ID поста
        await Groups.AddToGroupAsync(Context.ConnectionId, postId.ToString());
    }

    public async Task LeavePostGroup(Guid postId)
    {
        // Удаляем подключение из группы по ID поста
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId.ToString());
    }

    public async Task SendPostCreated(Guid postId, PostDto post)
    {
        // Отправляем сообщение о создании поста в соответствующую группу
        await Clients.Group(postId.ToString()).SendAsync("ReceivePostCreated", post);
    }

    public async Task SendPostUpdated(Guid postId, PostDto post)
    {
        // Отправляем сообщение о обновлении поста в соответствующую группу
        await Clients.Group(postId.ToString()).SendAsync("ReceivePostUpdated", post);
    }

    public async Task SendPostDeleted(Guid postId, PostDto post)
    {
        // Отправляем сообщение о удалении поста в соответствующую группу
        await Clients.Group(postId.ToString()).SendAsync("ReceivePostDeleted", post);
    }

    // Уведомления для конкретного пользователя
    public async Task NotifyUserPostCreated(Guid userId, PostDto post)
    {
        // Отправляем уведомление конкретному пользователю о создании поста
        await Clients.User(userId.ToString()).SendAsync("ReceiveUserPostCreated", post);
    }

    public async Task NotifyUserPostUpdated(Guid userId, PostDto post)
    {
        // Отправляем уведомление конкретному пользователю об обновлении поста
        await Clients.User(userId.ToString()).SendAsync("ReceiveUserPostUpdated", post);
    }
    
    public async Task  NotifyPostLikeCreated(PostDto post,string userId)
    {
        // Отправляем событие всем подключенным клиентам о лайке
        await Clients.Client(userId).SendAsync("ReceivePostLiked", post);
    }

    // Событие для удаления лайка
    public async Task NotifyPostLikeDeleted(PostDto post,string userId)
    {
        // Отправляем событие всем подключенным клиентам о снятии лайка
        await Clients.Client(userId).SendAsync("ReceivePostUnLiked", post);
    }
    public async Task UpdateLikesCount(PostDto postDto)
    {
        // Обработчик для обновления лайков на всех клиентах
        // Например, вы можете обновить интерфейс на клиентской стороне
        await Clients.All.SendAsync("UpdateLikesCount", postDto);
        
    }
    public async Task SendPostLikedStatus(Guid postId, Guid userId, bool isLiked)
    {
        // Отправляем обновленный статус лайка только конкретному пользователю
        await Clients.User(userId.ToString()).SendAsync("ReceivePostLikedStatus", postId, isLiked);
    }
    public async Task  NotifyCommentLikeCreated(CommentDto comment,string userId)
    {
        await Clients.All.SendAsync("ReceiveCommentLikedStatus", new CommentDto
        {
            Id = comment.Id,
            LikesCount = comment.LikesCount
        });
        // Отправляем событие всем подключенным клиентам о лайке
        await Clients.Client(userId).SendAsync("ReceiveCommentLikedStatus", comment);
    }

    // Событие для удаления лайка
    public async Task NotifyCommentLikeDeleted(CommentDto comment,string userId)
    {
        await Clients.All.SendAsync("ReceiveCommentUnLiked", new CommentDto
        {
            Id = comment.Id,
            LikesCount = comment.LikesCount
        });
        // Отправляем событие всем подключенным клиентам о снятии лайка
        await Clients.Client(userId).SendAsync("ReceiveCommentUnLiked", comment);
    }
    public async Task UpdateCommentLikesCount(CommentDto commentDto)
    {
        // Обработчик для обновления лайков на всех клиентах
        // Например, вы можете обновить интерфейс на клиентской стороне
        await Clients.All.SendAsync("UpdateLikesCount", commentDto);
        
    }
    public async Task SendCommentUpdated(Guid postId, CommentDto updatedComment)
    {
        await Clients.Group(postId.ToString()).SendAsync("ReceiveCommentUpdated", updatedComment);
    }

    public async Task NotifyUsersStatusUpdated(Guid userId, bool isOnline)
    {
        await Clients.All.SendAsync("UsersStatusChanged", userId, isOnline);
    }

    public async Task NotifyNewUser()
    {
        await Clients.All.SendAsync("UserListUpdated");
    }

    
}

