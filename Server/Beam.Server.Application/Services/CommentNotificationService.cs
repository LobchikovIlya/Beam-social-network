using Beam.Application.Services.Interfaces;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Application.Services;

public class CommentNotificationService : ICommentNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public CommentNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Уведомление о создании комментария
    public async Task NotifyCommentCreated(CommentDto comment)
    {
        // Отправка уведомления в группу по PostId
        await _hubContext.Clients.Group(comment.PostId.ToString()).SendAsync("ReceiveCommentCreated", comment);
    }

    // Уведомление об обновлении комментария
    public async Task NotifyCommentUpdated(CommentDto comment)
    {
        // Отправка уведомления в группу по PostId
        await _hubContext.Clients.Group(comment.PostId.ToString()).SendAsync("ReceiveCommentUpdated", comment);
    }

    // Уведомление об удалении комментария
    public async Task NotifyCommentDeleted(CommentDto comment)
    {
        // Отправка уведомления в группу по PostId
        await _hubContext.Clients.Group(comment.PostId.ToString()).SendAsync("ReceiveCommentDeleted", comment);
    }
}