using Bonfire.Application.Interfaces;
using Bonfire.Application.Services;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class RegisterServiceTests
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

    [Fact(DisplayName = "После успешной регистрации должен возвращаться токен авторизации.")]
    public async void Register_Should_Return_Token_If_Nickname_Is_Free()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var user = CreateUser();
        var request = new RegisterRequest { NickName = user.Nickname, Password = "test" };
        var passwordHasherService = A.Fake<IPasswordHasherService>();
        A.CallTo(() => passwordHasherService.HashPassword("test")).WithAnyArguments()
            .Returns("$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq");
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var context = new AppDbContext(options);
        var registerService = new RegisterService(context, tokenService);

        // Act
        var result = await registerService.Register(request);

        // Assert
        result.Should().Be("Token");
    }

    [Fact(DisplayName = "Регистрация выдает ошибку, если такой никнейм занят.")]
    public async void Register_Should_Give_Error_If_Nickname_Is_Not_Free()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        var user = CreateUser();
        var request = new RegisterRequest { NickName = user.Nickname, Password = "test" };
        
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var context = new AppDbContext(options);
        var registerService = new RegisterService(context, tokenService);
        
        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = async () => { await registerService.Register(request); };

        // Assert
        await result.Should().ThrowAsync<NicknameAlreadyExistsException>();
    }

    [Fact(DisplayName = "Регистрация выдает ошибку, если никнейм или пароль пустые")]
    public async void Register_Should_Give_Error_If_Data_Empty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var request = new RegisterRequest { NickName = string.Empty, Password = string.Empty };
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(new User())).WithAnyArguments().Returns("Token");
        var context = new AppDbContext(options);
        var registerService = new RegisterService(context, tokenService);

        // Act
        var result = async () => { await registerService.Register(request); };

        // Assert
        await result.Should().ThrowAsync<InvalidRegistrationDataException>();
    }

    [Fact(DisplayName = "Проверка существования юзеров возращает TRUE, если юзер существует")]
    public async void Check_User_Exists_Returns_True_If_User_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var user = CreateUser();
        var request = new RegisterRequest { NickName = user.Nickname, Password = "test" };
        
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var context = new AppDbContext(options);
        var registerService = new RegisterService(context, tokenService);
        await context.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await registerService.CheckUserExists(user.Nickname);


        // Assert
        result.Should().BeTrue();
    }


    [Fact(DisplayName = "Проверка существования юзеров возращает FALSE, если юзера не существует")]
    public async void Check_User_Exists_Returns_False_If_User_Does_Not_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var user = CreateUser();
        var request = new RegisterRequest { NickName = user.Nickname, Password = "test" };
        var passwordHasherService = A.Fake<IPasswordHasherService>();
        A.CallTo(() => passwordHasherService.HashPassword("test")).WithAnyArguments()
            .Returns("$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq");
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var context = new AppDbContext(options);
        var registerService = new RegisterService(context, tokenService);

        // Act
        var result = await registerService.CheckUserExists(user.Nickname);

        // Assert
        result.Should().BeFalse();
    }
}