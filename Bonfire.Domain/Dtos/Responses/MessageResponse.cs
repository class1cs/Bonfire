namespace Bonfire.Domain.Dtos.Responses;

public record MessageResponse(
    long Id,
    UserResponse Author,
    string Text,
    DateTimeOffset SentTime);