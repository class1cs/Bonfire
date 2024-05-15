namespace Bonfire.Core.Dtos.Response;

public class MessageResponseDto(Guid id, string text, UserResponseDto author)
{
    public Guid Id { get; private set; } = id;
    
    public string Text { get; private set; } = text;

    public UserResponseDto Author { get; private set; } = author;
}