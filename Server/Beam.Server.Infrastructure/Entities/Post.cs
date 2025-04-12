namespace Beam.Infrastructure.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public string UserName { get; set; }
    public DateTimeOffset CreationDate { get; set; }

    public virtual User User { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}