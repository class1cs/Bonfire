using Bonfire.Domain.Entities;

namespace Bonfire.Domain.Dtos.Responses;

public record ConversationResponse(
    long ConversationId,
    UserResponse[] Participants,
    ConversationType ConversationType);