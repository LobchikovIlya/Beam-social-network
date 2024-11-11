namespace Beam.Application.Filters;

public record PostLikeFilter
{
    public Guid? PostId { get; set; } = null;
    public Guid? UserId { get; set; } = null;
}