namespace Bonfire.Domain.Dtos.Responses;

public record MessagesResponse(
    long ConversationId, 
    MessageResponse[] Messages);