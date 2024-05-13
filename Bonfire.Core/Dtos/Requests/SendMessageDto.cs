namespace Bonfire.Core.Dtos.Requests;

public class SendMessageDto(string text)
{
    public string Text { get; private set; } = text;
}