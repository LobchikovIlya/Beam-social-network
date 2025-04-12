namespace Beam.Application.Services.Interfaces;

public interface IUserActivityService
{
    Task UpdateActivityAsync(Guid userId, DateTimeOffset lastActivityTime);
}