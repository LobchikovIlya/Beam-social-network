using Beam.Infrastructure;
using Beam.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class UserActivityMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5); // Тайм-аут неактивности
    private readonly IHubContext<ChatHub> _hubContext;
   

    public UserActivityMonitorService(IServiceProvider serviceProvider, IHubContext<ChatHub> hubContext)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeamDbContext>();

                var inactiveUsers = await dbContext.Users
                    .Where(u => u.IsOnline && u.LastActivity < DateTimeOffset.UtcNow - _timeout)
                    .ToListAsync();
                foreach (var user in inactiveUsers)
                {
                    user.IsOnline = false;
                }
                if (inactiveUsers.Any())
                {
                    await dbContext.SaveChangesAsync();
                    foreach (var user in inactiveUsers)
                    {
                        await _hubContext.Clients.All.SendAsync("UsersStatusChanged",user.Id , false);
                       
                    }
                    
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
