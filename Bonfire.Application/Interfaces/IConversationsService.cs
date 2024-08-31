using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IConversationsService
{
    Task<ConversationDto> CreateConversation(ConversationRequestDto conversationRequestDto);
    
    Task<ConversationDto[]> GetConversations(long offsetMessageId, short limit);
}