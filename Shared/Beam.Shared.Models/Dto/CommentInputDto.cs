namespace Beam.Shared.Dto;

public class CommentInputDto
{
    public string Author { get; set; }
    public string Content { get; set; }
    public Guid UserId { get; set; }
}