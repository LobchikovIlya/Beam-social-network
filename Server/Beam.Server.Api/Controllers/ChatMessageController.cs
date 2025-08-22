using Beam.Application.Services.Interfaces;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[ApiController]
[Route("api/")]
public class ChatMessageController : ControllerBase
{
    private readonly IChatMessageService _chatMessageService;

    public ChatMessageController(IChatMessageService chatMessageService)
    {
        _chatMessageService = chatMessageService;
    }

    [HttpGet("conversation")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetConversation([FromQuery] Guid senderId,
        [FromQuery] Guid receiverId)
    {
        var messages = await _chatMessageService.GetConversationAsync(senderId, receiverId);
        return Ok(messages.OrderBy(m => m.CreationDate).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChatMessageDto>> GetById(Guid id)
    {
        try
        {
            var message = await _chatMessageService.GetByIdAsync(id);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ChatMessageInputDto input)
    {
        try
        {
            await _chatMessageService.UpdateAsync(id, input);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _chatMessageService.DeleteByIdAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}