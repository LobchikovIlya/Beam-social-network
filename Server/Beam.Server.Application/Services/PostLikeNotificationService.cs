using Beam.Application.Services.Interfaces;
using Beam.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Application.Services;

public class PostLikeNotificationService : IPostLikeNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public PostLikeNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyPostLikeCreated(Guid postId)
    {
        await _hubContext.Clients.All.SendAsync("ReceivePostLiked", postId);
    }

    public async Task NotifyPostLikeDeleted(Guid postId)
    {
        await _hubContext.Clients.All.SendAsync("ReceivePostUnLiked", postId);
    }
}