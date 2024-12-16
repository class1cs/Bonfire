using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IConversationsService _conversationsService;

    private readonly IMessagesService _messagesService;

    public ChatController(IMessagesService messagesService, IConversationsService conversationsService)
    {
        _messagesService = messagesService;
        _conversationsService = conversationsService;
    }

    [HttpPost(Routes.Chat.SendMessage)]
    public async Task<IActionResult> SendMessage(
        long conversationId,
        [FromBody] MessageRequestDto messageRequestDto,
        CancellationToken cancellationToken)
    {
        var responseDto = await _messagesService.SendMessage(messageRequestDto, conversationId, cancellationToken);

        return Ok(responseDto);
    }

    [HttpGet(Routes.Chat.GetMessages)]
    public async Task<IActionResult> GetMessages(
        CancellationToken cancellationToken,
        long conversationId,
        short limit = 50,
        long offsetMessageId = 0)
    {
        var responseDto = await _messagesService.GetMessages(cancellationToken, conversationId, offsetMessageId, limit);

        return Ok(responseDto);
    }

    [HttpPut(Routes.Chat.EditMessage)]
    public async Task<IActionResult> EditMessage(
        long messageId,
        long conversationId,
        [FromBody] MessageRequestDto messageRequestDto,
        CancellationToken cancellationToken)
    {
        var responseDto = await _messagesService.EditMessage(messageRequestDto, messageId, conversationId, cancellationToken);

        return Ok(responseDto);
    }

    [HttpDelete(Routes.Chat.RemoveMessage)]
    public async Task<IActionResult> RemoveMessage(
        long messageId,
        long conversationId,
        CancellationToken cancellationToken)
    {
        var responseDto = await _messagesService.RemoveMessage(messageId, conversationId, cancellationToken);

        return Ok(responseDto);
    }

    [HttpPost(Routes.Chat.CreateConversation)]
    public async Task<IActionResult> CreateConversation(
        [FromBody] ConversationRequestDto conversationRequestDto,
        CancellationToken cancellationToken)
    {
        var responseDto = await _conversationsService.CreateConversation(conversationRequestDto, cancellationToken);

        return Ok(responseDto);
    }

    [HttpGet(Routes.Chat.GetConversations)]
    public async Task<IActionResult> GetConversations(
        CancellationToken cancellationToken,
        short limit = 50,
        long offsetConversationId = 0)
    {
        var responseDto = await _conversationsService.GetConversations(cancellationToken, offsetConversationId, limit);

        return Ok(responseDto);
    }
}