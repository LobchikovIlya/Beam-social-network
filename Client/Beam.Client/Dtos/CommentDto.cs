namespace Beam.UI.Dtos;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public int LikesCount { get; set; }

    public bool IsLiked { get; set; }

    public DateTimeOffset CreationDate { get; set; }
}