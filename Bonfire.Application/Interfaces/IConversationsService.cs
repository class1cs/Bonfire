using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IConversationsService
{
    Task<ConversationResponse> CreateConversation(ConversationRequest conversationRequest, CancellationToken cancellationToken);

    Task<ConversationResponse[]> GetConversations(
        CancellationToken cancellationToken,
        long offsetMessageId = 0,
        short limit = 50);
}