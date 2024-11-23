namespace Bonfire.Domain.Dtos.Responses;

public class TokenDto
{
    public TokenDto(string accessToken, DateTime expiresAt)
    {
        AccessToken = accessToken;
        ExpiresAt = expiresAt;
    }

    public string AccessToken { get; private set; }
    
    public DateTime ExpiresAt { get; private set; }
}