using Bonfire.Application.Services;
using FakeItEasy;
using FluentAssertions;

namespace Bonfire.Tests;

public class PasswordHasherServiceTests
{
    [Fact(DisplayName = "Пароль должен хешироваться.")]
    public void Password_Should_Be_Hashed()
    {
        // Arrange
        var hasher = A.Fake<PasswordHasherService>();
        // Act
        var hash = hasher.HashPassword("test");

        // Assert
        var verifyHash = BCrypt.Net.BCrypt.EnhancedVerify("test", hash);
        verifyHash.Should().BeTrue();
    }
}