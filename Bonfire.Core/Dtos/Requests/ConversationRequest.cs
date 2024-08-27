namespace Bonfire.Core.Dtos.Requests;

public class ConversationRequest
{
    public required List<long> UsersIds { get; set; }
}