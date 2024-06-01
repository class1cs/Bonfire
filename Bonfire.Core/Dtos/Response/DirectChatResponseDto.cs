using Bonfire.Core.Entities;

namespace Bonfire.Core.Dtos.Response;

public class DirectChatResponseDto
{
    public Guid Id { get; private set; }

    public List<MessageResponseDto> ChatHistory { get; private set; }
    
    public List<UserResponseDto> Participants { get; private set; }
}