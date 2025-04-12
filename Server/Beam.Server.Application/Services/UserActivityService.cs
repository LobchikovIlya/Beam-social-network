using Beam.Application.Services.Interfaces;
using Beam.Infrastructure;
using Beam.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Beam.Application.Services;

public class UserActivityService : IUserActivityService
{
    private readonly BeamDbContext _dbContext;
    private readonly IHubContext<ChatHub> _hubContext;

    public UserActivityService(BeamDbContext dbContext, IHubContext<ChatHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    public async Task UpdateActivityAsync(Guid userId, DateTimeOffset lastActivityTime)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastActivity = lastActivityTime;
            user.IsOnline = true;
            await _dbContext.SaveChangesAsync();

            // Отправляем обновление через SignalR
            await _hubContext.Clients.All.SendAsync("UsersStatusChanged", user.Id, true);
        }
    }
}