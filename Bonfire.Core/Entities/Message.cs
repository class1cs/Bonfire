namespace Bonfire.Core.Entities;

public class Message(Guid id, string text, User user, DateTime sentTime)
{
    public Guid Id { get; private set; } = id;

    public string Text { get; private set; } = text;

    public DateTime SentTime { get; private set; } = sentTime;
    
    public User Author { get; private set; } = user;
}