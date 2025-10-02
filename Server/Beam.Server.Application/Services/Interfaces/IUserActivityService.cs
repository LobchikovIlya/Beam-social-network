namespace Beam.Application.Services.Interfaces;

public interface IUserActivityService
{
    Task UpdateActivityAsync(Guid userId, DateTimeOffset lastActivityTime);
    Task SetOnlineStatusAsync(Guid userId, bool isOnline);
    Task UpdateLastActivityAsync(Guid userId);
}