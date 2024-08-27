namespace Bonfire.Core.Dtos.Requests;

public class LoginRequest
{
    public required string NickName { get; set; }

    public required string Password { get; set; }
}