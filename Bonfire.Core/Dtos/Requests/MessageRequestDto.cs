namespace Bonfire.Core.Dtos.Requests;

public class MessageRequestDto(string text)
{
    public string Text { get; private set; } = text;
}