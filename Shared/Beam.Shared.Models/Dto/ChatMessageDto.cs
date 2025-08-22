namespace Beam.Shared.Dto;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Text { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}