namespace Beam.Shared.Dto;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    
    public string UserName { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    
    public int LikesCount { get; set; }
    
    public bool IsLiked { get; set; }
}