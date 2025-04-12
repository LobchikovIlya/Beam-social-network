namespace Beam.Infrastructure.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public bool IsOnline { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public ICollection<UsersToRole> UsersToRoles { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<CommentLike> CommentLikes { get; set; }
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
}
    
