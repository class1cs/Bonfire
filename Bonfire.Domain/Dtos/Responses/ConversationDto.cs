using Bonfire.Domain.Entities;

namespace Bonfire.Domain.Dtos.Responses;

public record ConversationDto(
    long ConversationId,
    UserDto[] Participants,
    ConversationType ConversationType);