using Bonfire.Core.Entities;

namespace Bonfire.Core.Dtos.Response;

public class MessagesResponse
{
    public required long Id { get; set; }
    
    public required MessageResponse[] Messages { get; set; }
}