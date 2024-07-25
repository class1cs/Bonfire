using Bonfire.Abstractions;
using Bonfire.Application.Services;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

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
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        
        var tokenService = new TokenService();

        await context.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = tokenService.GenerateToken(user);
        
        
        // Assert
        result.Should().NotBeNullOrWhiteSpace();

    }
}