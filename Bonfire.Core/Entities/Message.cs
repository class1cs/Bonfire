using System.ComponentModel.DataAnnotations.Schema;

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

    public long Id { get; set; }

    public string Text { get; set; }

    public DateTime SentTime { get; set; }
    
    public User Author { get; set; }
    
    public Conversation Conversation { get; set; }
    
    public long ConversationId { get; set; }
    
}