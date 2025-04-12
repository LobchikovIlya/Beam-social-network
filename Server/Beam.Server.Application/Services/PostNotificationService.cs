using Beam.Application.Services.Interfaces;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Application.Services;

public class PostNotificationService : IPostNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;
    public PostNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Уведомление о создании поста
    public async Task NotifyPostCreated(PostDto post)
    {
        // Отправка уведомления в группу по PostId
        await _hubContext.Clients.All.SendAsync("ReceivePostCreated", post);
    }

    // Уведомление об обновлении поста
    public async Task NotifyPostUpdated(PostDto post)
    {
        // Отправка уведомления в группу по PostId
        await _hubContext.Clients.All.SendAsync("ReceivePostUpdated", post);
    }

    // Уведомление об удалении поста
    public async Task NotifyPostDeleted(PostDto post)
    {
        // Отправка уведомления в группу по PostId
        await _hubContext.Clients.All.SendAsync("ReceivePostDeleted", post);
    }
    public async Task NotifyUserPostCreated(Guid userId, PostDto post)
    {
        // Уведомление конкретного пользователя о создании поста
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveUserPostCreated", post);
    }

    public async Task NotifyUserPostUpdated(Guid userId, PostDto post)
    {
        // Уведомление конкретного пользователя о обновлении поста
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveUserPostUpdated", post);
    }
    public async Task NotifyUserPostDeleted(Guid userId, PostDto post)
    {
        // Отправка уведомления конкретному пользователю о удалении поста
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveUserPostDeleted", post);
    }
}
