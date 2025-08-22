using Beam.Shared.Dto;

namespace Beam.Application.Services.Interfaces;

public interface IChatMessageService
{
    Task<List<ChatMessageDto>> GetConversationAsync(Guid senderId, Guid receiverId);
    Task<ChatMessageDto> GetByIdAsync(Guid id);
    Task UpdateAsync(Guid id, ChatMessageInputDto input);
    Task DeleteByIdAsync(Guid id);
}