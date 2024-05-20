namespace Bonfire.Core.Entities;

public class Message(Guid id, string text, User user, DateTime sentTime)
{
    public Guid Id { get; set; } = id;

    public string Text { get; set; } = text;

    public DateTime SentTime { get; set; } = sentTime;
    
    public User Author { get; set; } = user;
}