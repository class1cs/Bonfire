namespace Bonfire.Core.Dtos.Response;

public class MessageResponse
{
    public required long Id { get; set; }

    public required UserResponse Author { get; set; }

    public required string Text { get; set; }

    public required DateTime SentTime { get; set; }
}