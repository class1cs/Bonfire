using Bonfire.Application.Interfaces;
using Bonfire.Core.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Route("api/[controller]/conversations/")]
[ApiController]
[Authorize]
public class ChatController(IMessagesService messagesService, IConversationsService conversationsService)
    : ControllerBase
{
    [HttpPost("{conversationId:long}/messages")]
    public async Task<IActionResult> SendMessage(long conversationId, [FromBody] MessageRequest messageRequest)
    {
        var responseDto = await messagesService.SendMessage(messageRequest, conversationId);
        return Ok(responseDto);
    }

    [HttpGet("{conversationId:long}/messages")]
    public async Task<IActionResult> GetMessages(long conversationId, short limit = 50, long offsetMessageId = 0)
    {
        var responseDto = await messagesService.GetMessages(conversationId, offsetMessageId, limit);
        return Ok(responseDto);
    }

    [HttpPut("{conversationId:long}/messages/{messageId:long}")]
    public async Task<IActionResult> EditMessage(long messageId, long conversationId,
        [FromBody] MessageRequest messageRequest)
    {
        var responseDto = await messagesService.EditMessage(messageRequest, messageId, conversationId);
        return Ok(responseDto);
    }

    [HttpDelete("{conversationId:long}/messages/{messageId:long}")]
    public async Task<IActionResult> RemoveMessage(long messageId, long conversationId)
    {
        var responseDto = await messagesService.RemoveMessage(messageId, conversationId);
        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateConversation([FromBody] ConversationRequest conversationRequest)
    {
        var responseDto = await conversationsService.CreateConversation(conversationRequest);
        return Ok(responseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations(short limit = 50, long offsetConversationId = 0)
    {
        var responseDto = await conversationsService.GetConversations(offsetConversationId, limit);
        return Ok(responseDto);
    }
}