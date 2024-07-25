using Bonfire.Abstractions;
using Bonfire.Application.Services;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class LoginServiceTests
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
    
    
   
    
    [Fact(DisplayName = "Проверка данных проходит успешно, если данные верны")]
    public async void Data_Check_Success_If_Data_Correct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var loginService =  new LoginService(tokenService, context);
     
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = await loginService.VerifyLoginCredentials(user.Nickname, "test");
        
        // Assert
        result.Id.Should().Be(user.Id);
    }
    
    [Fact(DisplayName = "После успешного входа должен возвращаться токен авторизации.")]
    public async void Login_Should_ReturnToken_If_Data_Correct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var loginService = new LoginService(tokenService, context);
        
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = await loginService.Login(new LoginRequest { NickName = user.Nickname, Password = "test" });
        
        // Assert
        result.Should().Be("Token");
    }
    
   
    
    [Fact(DisplayName = "При входе с неверным логином и паролем должно выдать ошибку авторизации.")]
    public async void Login_Should_Fail_If_Login_Data_Incorrect()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
       
        
        var tokenService = A.Fake<ITokenService>();
        A.CallTo(() => tokenService.GenerateToken(user)).WithAnyArguments().Returns("Token");
        var loginService = new LoginService(tokenService, context);
        
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = async () =>
        {
            await loginService.Login(new LoginRequest
            {
                NickName = string.Empty,
                Password = string.Empty
            });
        };
        
        // Assert
        await result.Should().ThrowAsync<InvalidLoginCredentialsException>();
        
    }
}