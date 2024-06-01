namespace Bonfire.Core.Entities;

public class Message
{
    public Message(string text, DateTime sentTime, User author)
    {
        Text = text;
        SentTime = sentTime;
        Author = author;
    }

    public Message()
    {
        
    }

    public Guid Id { get; set; }

    public string Text { get; set; }

    public DateTime SentTime { get; set; }
    
    public User Author { get; set; }
}