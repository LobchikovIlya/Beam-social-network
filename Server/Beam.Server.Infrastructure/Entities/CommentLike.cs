namespace Beam.Infrastructure.Entities;

public class CommentLike
{
    public Guid UserId { get; set; }
    public Guid CommentId { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    
    public virtual Comment Comment { get; set; } 
    public virtual User User { get; set; }
}