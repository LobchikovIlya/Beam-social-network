namespace Beam.UI.Dtos;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Content { get; set; }
    public int LikesCount { get; set; }

    public bool IsLiked { get; set; }

    public DateTimeOffset CreationDate { get; set; }
    public int CommentCount { get; set; }
    public List<CommentDto> Comments { get; set; }
}