using Bonfire.Application.Helpers;
using FluentAssertions;

namespace Bonfire.Tests;

public class PasswordHasherTests
{
    [Fact(DisplayName = "Пароль должен хешироваться.")]
    public void Password_Should_Be_Hashed()
    {
        // Act
        var hash = PasswordHasher.HashPassword("test");

        // Assert
        var verifyHash = BCrypt.Net.BCrypt.EnhancedVerify("test", hash);
        verifyHash.Should().BeTrue();
    }
}