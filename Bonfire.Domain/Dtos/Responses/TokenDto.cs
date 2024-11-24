namespace Bonfire.Domain.Dtos.Responses;

public class TokenDto
{
    public TokenDto(string accessToken, DateTimeOffset expiresAt)
    {
        AccessToken = accessToken;
        ExpiresAt = expiresAt;
    }

    public string AccessToken { get; private set; }
    
    public DateTimeOffset ExpiresAt { get; private set; }
}