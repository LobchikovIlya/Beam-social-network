using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface IPostNotificationService
{
    Task NotifyPostCreated(PostDto post);
    Task NotifyPostUpdated(PostDto post);
    Task NotifyPostDeleted(PostDto post);
    Task NotifyUserPostCreated(Guid userId, PostDto post);
    Task NotifyUserPostUpdated(Guid userId, PostDto post);
    Task NotifyUserPostDeleted(Guid userId, PostDto post);
}