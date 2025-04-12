using  Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface ICommentNotificationService
{
    Task NotifyCommentCreated(CommentDto comment);
    Task NotifyCommentUpdated(CommentDto comment);
    Task NotifyCommentDeleted(CommentDto comment);
}