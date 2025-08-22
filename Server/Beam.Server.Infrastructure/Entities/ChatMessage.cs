namespace Beam.Infrastructure.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}