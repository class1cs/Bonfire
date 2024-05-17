namespace Bonfire.Core.Entities;

public class DirectChat(Guid id, List<Message> chatHistory, List<User> participants)
{
    public Guid Id { get; private set; } = id;

    public List<Message> ChatHistory { get; private set; } = chatHistory;
    
    public List<User> Participants { get; private set; } = participants;
}