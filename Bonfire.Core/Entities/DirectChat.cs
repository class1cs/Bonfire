namespace Bonfire.Core.Entities;

public class DirectChat(Guid id, List<Message> chatHistory, List<User> participants)
{
    public Guid Id { get; set; } = id;

    public List<Message> ChatHistory { get; set; } = chatHistory;
    
    public List<User> Participants { get; set; } = participants;
}