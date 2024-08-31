namespace Bonfire.Domain.Dtos.Responses;

public record MessagesDto(long ConversationId, MessageDto[] Messages);