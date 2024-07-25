namespace Bonfire.Core.Dtos.Requests;

public class RegisterRequest
{
    public required string NickName { get; set; }

    public required string Password { get; set; }
}