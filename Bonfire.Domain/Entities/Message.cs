namespace Bonfire.Domain.Entities;

public class Message
{
    public Message(string text, User author)
    {
        Text = text;
        Author = author;
    }

    public Message()
    {
    }

    public long Id { get; set; }

    public string Text { get; set; }

    public DateTime SentTime { get; } = DateTime.Now.ToLocalTime();

    public User Author { get; set; }

    public Conversation Conversation { get; set; }

    public long ConversationId { get; set; }
}