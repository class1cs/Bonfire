namespace Bonfire.Core.Entities;

public class Message(Guid id, string text, DateTime sentTime)
{
    public Guid Id { get; private set; } = id;

    public string Text { get; private set; } = text;

    public DateTime SentTime { get; private set; } = sentTime;
}