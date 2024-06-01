namespace Bonfire.Core.Entities;

public class DirectChat
{
    public DirectChat(List<Message> chatHistory, List<User> participants)
    {
        ChatHistory = chatHistory;
        Participants = participants;
    }

    public DirectChat()
    {
        
    }
    
    public Guid Id { get; set; }

    public List<Message> ChatHistory { get; set; }
    
    public List<User> Participants { get; set; }
}