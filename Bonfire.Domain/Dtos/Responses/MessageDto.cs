namespace Bonfire.Domain.Dtos.Responses;

public record MessageDto(long Id, UserDto Author, string Text, DateTimeOffset SentTime);