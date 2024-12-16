using System.Text;
using Bonfire.Application.Services;
using Bonfire.Domain.Entities;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IConfiguration = Castle.Core.Configuration.IConfiguration;

namespace Bonfire.Tests;

public class TokenServiceTests
{
    private User CreateUser(string name = "test", long id = 1)
    {
        var user = new User();
        user.Id = id;
        user.Nickname = name;
        user.Conversations = new List<Conversation>();
        user.PasswordHash = "$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq";

        return user;
    }

    [Fact(DisplayName = "Возврат токена при передаче юзера.")]
    public async void Login_Should_Fail_If_Login_Data_Incorrect()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var context = new AppDbContext(options);

        var user = CreateUser();
        var timeProvider = A.Fake<TimeProvider>();

        var fakeConfiguration = A.Fake<IConfiguration>();

        var appSettings = """
                          {"AuthOptions": {
                            "Issuer": "Bonfire",
                            "Audience": "BelovedUser",
                            "Key": "SecretKeyForBonfire1333337777777",
                            "AccessTokenValidityInDays": "2"
                          }}
                          """;

        var builder = new ConfigurationBuilder();
        builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
        var configuration = builder.Build();

        var tokenService = new TokenService(configuration, timeProvider);

        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = tokenService.GenerateToken(user);

        // Assert
        result.AccessToken.Should()
            .NotBeNullOrWhiteSpace();

        result.ExpiresAt.Should()
            .NotBe(null);
    }
}