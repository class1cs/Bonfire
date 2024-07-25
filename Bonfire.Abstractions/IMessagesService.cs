using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;

namespace Bonfire.Abstractions;

public interface IMessagesService
{
    Task<MessageResponse> SendMessage(MessageRequest messageRequest, long conversationId);
    
    Task<MessageResponse> EditMessage(MessageRequest messageRequest, long messageId, long conversationId);
    
    Task<MessageResponse> RemoveMessage(long messageId, long conversationId);
    
    Task<MessagesResponse> GetMessages(long conversationId, long offsetMessageId = 1, short limit = 50);
}