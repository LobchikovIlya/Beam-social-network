namespace Beam.Application.Services.Interfaces;

public interface IPostLikeNotificationService
{
    Task NotifyPostLikeCreated(Guid postId);
    Task NotifyPostLikeDeleted(Guid postId);
}