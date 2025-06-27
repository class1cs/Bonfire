namespace Bonfire.Domain.Dtos.Responses;

public record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);