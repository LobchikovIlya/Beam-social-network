namespace Beam.Infrastructure.Entities;

public class PostLike
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}