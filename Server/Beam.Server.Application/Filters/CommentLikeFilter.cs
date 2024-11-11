namespace Beam.Application.Filters;

public record CommentLikeFilter
{
    public Guid? UserId { get; set; } = null;
    public Guid? CommentId { get; set; } = null;
}