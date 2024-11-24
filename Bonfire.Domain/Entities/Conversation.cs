using System.ComponentModel.DataAnnotations;

namespace Bonfire.Domain.Entities;

public class Conversation
{
    public Conversation(ICollection<Message> messages, ICollection<User> participants, ConversationType type)
    {
        Messages = messages;
        Participants = participants;
        Type = type;
    }

    public Conversation()
    {
    }
    
    
    public long Id { get; set; }

    public ICollection<Message> Messages { get; set; }

    public ICollection<User> Participants { get; set; }

    public ConversationType Type { get; set; }
}

public enum ConversationType
{
    Dialogue = 1,
    Conversation = 2
}