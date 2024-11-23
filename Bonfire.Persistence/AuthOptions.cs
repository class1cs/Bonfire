using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bonfire.Persistance;

public class AuthOptions
{
    public const string Issuer = "Bonfire";

    public const string Audience = "BelovedUser";
    
    private const string Key = "SecretKeyForBonfire1333337777777";

    public static readonly DateTime AccessTokenValidity = DateTime.UtcNow.Add(TimeSpan.FromMinutes(1));
    
    public static readonly DateTime RefreshTokenValidity = DateTime.UtcNow.Add(TimeSpan.FromDays(7));
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}