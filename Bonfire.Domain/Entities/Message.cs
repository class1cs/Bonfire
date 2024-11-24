using System.ComponentModel.DataAnnotations;

namespace Bonfire.Domain.Entities;

public class Message
{
    public Message(string text, User author, DateTimeOffset sentTime)
    {
        Text = text;
        Author = author;
        SentTime = sentTime;
    }

    public Message()
    {
    }
    
    public long Id { get; set; }

    public string Text { get; set; }

    public DateTimeOffset SentTime { get; } 

    public User Author { get; set; }

    public Conversation Conversation { get; set; }

    public long ConversationId { get; set; }
}