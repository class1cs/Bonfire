using Bonfire.Core.Entities;

namespace Bonfire.Core.Dtos.Response;

public class DirectChatResponseDto(Guid id, List<UserResponseDto> participants, List<MessageResponseDto> chatHistory)
{
    public Guid Id { get; private set; } = id;

    public List<MessageResponseDto> ChatHistory { get; private set; } = chatHistory;
    
    public List<UserResponseDto> Participants { get; private set; } = participants;
}