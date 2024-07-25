using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;

namespace Bonfire.Abstractions;

public interface IConversationsService
{
    Task<ConversationResponse> CreateConversation(ConversationRequest conversationRequest);
    Task<ConversationResponse> ExitConversation(long conversationId);
}