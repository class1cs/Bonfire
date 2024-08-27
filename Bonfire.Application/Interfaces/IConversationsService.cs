using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;

namespace Bonfire.Application.Interfaces;

public interface IConversationsService
{
    Task<ConversationResponse> CreateConversation(ConversationRequest conversationRequest);
    Task<ConversationResponse[]> GetConversations(long offsetMessageId, short limit);
}