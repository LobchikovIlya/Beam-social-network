using Beam.Application.Services.Interfaces;
using Beam.Infrastructure;
using Beam.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class ChatMessageService : IChatMessageService
{
    private readonly BeamDbContext _dbContext;

    public ChatMessageService(BeamDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ChatMessageDto>> GetConversationAsync(Guid senderId, Guid receiverId)
    {
        var messages = await _dbContext.ChatMessages
            .Where(m =>
                (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
        return messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Text = m.Text,
            CreationDate = m.Timestamp
        }).ToList();
    }

    public async Task<ChatMessageDto> GetByIdAsync(Guid id)
    {
        var message = await _dbContext.ChatMessages.FindAsync(id);
        if (message == null)
            throw new Exception($"Message with id {id} not found");

        return new ChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Text = message.Text,
            CreationDate = message.Timestamp
        };
    }

    public async Task UpdateAsync(Guid id, ChatMessageInputDto input)
    {
        var message = await _dbContext.ChatMessages.FindAsync(id);
        if (message == null) throw new Exception($"Message with id {id} not found");
        message.Text = input.Text;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var message = await _dbContext.ChatMessages.FindAsync(id);
        if (message == null) throw new Exception($"Message with id {id} not found");
        _dbContext.ChatMessages.Remove(message);
        await _dbContext.SaveChangesAsync();
    }
}