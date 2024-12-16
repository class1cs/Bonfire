using Bonfire.Application.Interfaces;
using Bonfire.Application.Services;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class IdentityServiceTests
{
    [Fact(DisplayName = "Проверка данных проходит успешно, если данные верны")]
    public async void Data_Check_Success_If_Data_Correct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var context = new AppDbContext(options);

        var user = CreateUser();

        var tokenService = A.Fake<ITokenService>();
        var timeProvider = A.Fake<TimeProvider>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(user))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var loginService = new IdentityService(tokenService, context);

        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await loginService.VerifyLoginCredentials(user.Nickname, "test", default);

        // Assert
        result!.Id.Should()
            .Be(user.Id);
    }

    [Fact(DisplayName = "После успешного входа должен возвращаться токен авторизации.")]
    public async void Login_Should_ReturnToken_If_Data_Correct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var context = new AppDbContext(options);

        var user = CreateUser();

        var tokenService = A.Fake<ITokenService>();
        var timeProvider = A.Fake<TimeProvider>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(user))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var loginService = new IdentityService(tokenService, context);

        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await loginService.Login(new(user.Nickname, "test"), default);

        // Assert
        result.AccessToken.Should()
            .Be("Token");

        result.ExpiresAt.Should()
            .Be(utcNow);
    }

    [Fact(DisplayName = "При входе с неверным логином и паролем должно выдать ошибку авторизации.")]
    public async void Login_Should_Fail_If_Login_Data_Incorrect()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var context = new AppDbContext(options);

        var user = CreateUser();

        var tokenService = A.Fake<ITokenService>();
        var timeProvider = A.Fake<TimeProvider>();

        A.CallTo(() => tokenService.GenerateToken(user))
            .WithAnyArguments()
            .Returns(new("Token", timeProvider.GetUtcNow()));

        var loginService = new IdentityService(tokenService, context);

        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = async () =>
        {
            await loginService.Login(new(string.Empty,
                string.Empty), default);
        };

        // Assert
        await result.Should()
            .ThrowAsync<InvalidLoginCredentialsException>();
    }

    private User CreateUser(string name = "test", long id = 1)
    {
        var user = new User
        {
            Id = id,
            Nickname = name,
            Conversations = new List<Conversation>(),
            PasswordHash = "$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq"
        };

        return user;
    }

    [Fact(DisplayName = "После успешной регистрации должен возвращаться токен авторизации.")]
    public async void Register_Should_Return_Token_If_Nickname_Is_Free()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var user = CreateUser();
        var request = new RegisterRequestDto(user.Nickname, "test");
        var passwordHasherService = A.Fake<IPasswordHasherService>();

        A.CallTo(() => passwordHasherService.HashPassword("test"))
            .WithAnyArguments()
            .Returns("$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq");

        var tokenService = A.Fake<ITokenService>();
        var timeProvider = A.Fake<TimeProvider>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(user))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var context = new AppDbContext(options);
        var identityService = new IdentityService(tokenService, context);

        // Act
        var result = await identityService.Register(request, default);

        // Assert
        result.AccessToken.Should()
            .Be("Token");

        result.ExpiresAt.Should()
            .Be(utcNow);
    }

    [Fact(DisplayName = "Регистрация выдает ошибку, если такой никнейм занят.")]
    public async void Register_Should_Give_Error_If_Nickname_Is_Not_Free()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var user = CreateUser();
        var request = new RegisterRequestDto(user.Nickname, "test");

        var tokenService = A.Fake<ITokenService>();
        var timeProvider = A.Fake<TimeProvider>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(user))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var context = new AppDbContext(options);
        var identityService = new IdentityService(tokenService, context);

        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = async () =>
        {
            await identityService.Register(request, default);
        };

        // Assert
        await result.Should()
            .ThrowAsync<NicknameAlreadyExistsException>();
    }

    [Fact(DisplayName = "Регистрация выдает ошибку, если никнейм или пароль пустые")]
    public async void Register_Should_Give_Error_If_Data_Empty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var request = new RegisterRequestDto(string.Empty, string.Empty);

        var timeProvider = A.Fake<TimeProvider>();
        var tokenService = A.Fake<ITokenService>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(new()))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var context = new AppDbContext(options);
        var identityService = new IdentityService(tokenService, context);

        // Act
        var result = async () =>
        {
            await identityService.Register(request, default);
        };

        // Assert
        await result.Should()
            .ThrowAsync<InvalidRegistrationDataException>();
    }

    [Fact(DisplayName = "Проверка существования юзеров возращает TRUE, если юзер существует")]
    public async void Check_User_Exists_Returns_True_If_User_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var user = CreateUser();

        var timeProvider = A.Fake<TimeProvider>();
        var tokenService = A.Fake<ITokenService>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(new()))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var context = new AppDbContext(options);
        var identityService = new IdentityService(tokenService, context);

        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await identityService.CheckUserExists(user.Nickname, default);

        // Assert
        result.Should()
            .BeTrue();
    }

    [Fact(DisplayName = "Проверка существования юзеров возращает FALSE, если юзера не существует")]
    public async void Check_User_Exists_Returns_False_If_User_Does_Not_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        var user = CreateUser();

        var timeProvider = A.Fake<TimeProvider>();
        var passwordHasherService = A.Fake<IPasswordHasherService>();

        A.CallTo(() => passwordHasherService.HashPassword("test"))
            .WithAnyArguments()
            .Returns("$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq");

        var tokenService = A.Fake<ITokenService>();
        var utcNow = timeProvider.GetUtcNow();

        A.CallTo(() => tokenService.GenerateToken(user))
            .WithAnyArguments()
            .Returns(new("Token", utcNow));

        var context = new AppDbContext(options);
        var identityService = new IdentityService(tokenService, context);

        // Act
        var result = await identityService.CheckUserExists(user.Nickname, default);

        // Assert
        result.Should()
            .BeFalse();
    }
}