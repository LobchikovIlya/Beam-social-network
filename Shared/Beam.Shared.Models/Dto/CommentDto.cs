namespace Beam.Shared.Dto;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
     
    public string Author { get; set; }
    
    public string Content { get; set; }
    
    public DateTimeOffset CreationDate { get; set; }
    
    public int LikesCount { get; set; } // Количество лайков
   
    public bool IsLiked { get; set; } // Лайкнут ли текущим пользователе
}