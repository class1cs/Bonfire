using Bonfire.Core.Entities;

namespace Bonfire.Core.Dtos.Response;

public class ConversationResponse
{
    public required long Id { get; set; }
    
    public required List<UserResponse> Participants { get; set; }
    
    public required ConversationType ConversationType { get; set; }
}