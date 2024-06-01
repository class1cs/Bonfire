using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bonfire.Persistance;

public class AuthOptions
{
    public const string Issuer = "Bonfire";

    public const string Audience = "BelovedUser";
    private const string Key = "ultrasecretkey_topsecret131111237";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}