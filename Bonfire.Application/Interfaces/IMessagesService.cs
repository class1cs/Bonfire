using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IMessagesService
{
    Task<MessageDto> SendMessage(
        MessageRequestDto messageRequestDto,
        long conversationId,
        CancellationToken cancellationToken);

    Task<MessageDto> EditMessage(
        MessageRequestDto messageRequestDto,
        long messageId,
        long conversationId,
        CancellationToken cancellationToken);

    Task<MessageDto> RemoveMessage(
        long messageId,
        long conversationId,
        CancellationToken cancellationToken);

    Task<MessagesDto> GetMessages(
        CancellationToken cancellationToken,
        long conversationId,
        long offsetMessageId = 1,
        short limit = 50);
}