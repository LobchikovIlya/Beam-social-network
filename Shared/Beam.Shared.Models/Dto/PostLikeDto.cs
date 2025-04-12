namespace Beam.Shared.Dto;

public class PostLikeDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}