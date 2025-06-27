using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IMessagesService
{
    Task<MessageResponse> SendMessage(
        MessageRequest messageRequest,
        long conversationId,
        CancellationToken cancellationToken);

    Task<MessageResponse> EditMessage(
        MessageRequest messageRequest,
        long messageId,
        long conversationId,
        CancellationToken cancellationToken);

    Task<MessageResponse> RemoveMessage(
        long messageId,
        long conversationId,
        CancellationToken cancellationToken);

    Task<MessagesResponse> GetMessages(
        CancellationToken cancellationToken,
        long conversationId,
        long offsetMessageId = 1,
        short limit = 50);
}