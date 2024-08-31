using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Route("api/[controller]/conversations/")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMessagesService _messagesService;
    private readonly IConversationsService _conversationsService;

    public ChatController(IMessagesService messagesService, IConversationsService conversationsService)
    {
        _messagesService = messagesService;
        _conversationsService = conversationsService;
    }
    
    [HttpPost("{conversationId:long}/messages")]
    public async Task<IActionResult> SendMessage(long conversationId, [FromBody] MessageRequestDto messageRequestDto)
    {
        var responseDto = await _messagesService.SendMessage(messageRequestDto, conversationId);
        return Ok(responseDto);
    }

    [HttpGet("{conversationId:long}/messages")]
    public async Task<IActionResult> GetMessages(long conversationId, short limit = 50, long offsetMessageId = 0)
    {
        var responseDto = await _messagesService.GetMessages(conversationId, offsetMessageId, limit);
        return Ok(responseDto);
    }

    [HttpPut("{conversationId:long}/messages/{messageId:long}")]
    public async Task<IActionResult> EditMessage(long messageId, long conversationId,
        [FromBody] MessageRequestDto messageRequestDto)
    {
        var responseDto = await _messagesService.EditMessage(messageRequestDto, messageId, conversationId);
        return Ok(responseDto);
    }

    [HttpDelete("{conversationId:long}/messages/{messageId:long}")]
    public async Task<IActionResult> RemoveMessage(long messageId, long conversationId)
    {
        var responseDto = await _messagesService.RemoveMessage(messageId, conversationId);
        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateConversation([FromBody] ConversationRequestDto conversationRequestDto)
    {
        var responseDto = await _conversationsService.CreateConversation(conversationRequestDto);
        return Ok(responseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations(short limit = 50, long offsetConversationId = 0)
    {
        var responseDto = await _conversationsService.GetConversations(offsetConversationId, limit);
        return Ok(responseDto);
    }
}