using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IConversationsService
{
    Task<ConversationDto> CreateConversation(ConversationRequestDto conversationRequestDto, CancellationToken cancellationToken);
    
    Task<ConversationDto[]> GetConversations(CancellationToken cancellationToken, long offsetMessageId, short limit);
}