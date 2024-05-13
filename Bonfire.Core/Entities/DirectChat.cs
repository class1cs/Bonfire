namespace Bonfire.Core.Entities;

public class DirectChat(Guid id, List<Message> chatHistory)
{
    public Guid Id { get; private set; } = id;

    public List<Message> ChatHistory { get; private set; } = chatHistory;
}