namespace Beam.Infrastructure.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }

    public string Author { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public string Content { get; set; }

    public virtual Post Post { get; set; }
    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
}