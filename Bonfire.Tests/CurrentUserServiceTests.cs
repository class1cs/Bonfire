using System.Security.Claims;
using Bonfire.Abstractions;
using Bonfire.Application.Services;
using Bonfire.Core.Entities;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class CurrentUserServiceTests
{
    public User CreateUser()
    {
        var user = A.Fake<User>();
        user.Id = 1;
        user.Nickname = "test";
        user.Conversations = new List<Conversation>();
        user.PasswordHash = "$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq";

        return user;
    }
    
    [Fact(DisplayName = "Должно возвращать текущего пользователя, если  токен верен.")]
    public async void Data_Check_Success_If_Data_Correct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        
        var context = new AppDbContext(options);
        
        var httpContextAccessor = A.Fake<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = A.Fake<HttpContext>();
        httpContextAccessor.HttpContext.User = A.Fake<ClaimsPrincipal>();
        var userIdClaim = A.Fake<Claim>(x => x.WithArgumentsForConstructor(() => new Claim("Id", "1")));
        A.CallTo(() => httpContextAccessor.HttpContext.User.Identity.IsAuthenticated).Returns(true);
        A.CallTo(() => httpContextAccessor.HttpContext.User.Claims).Returns(new List<Claim> { userIdClaim });
        
        
        var user = CreateUser();
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        

        var currentUserService = new CurrentUserService(httpContextAccessor, context);
        
        // Act
        var result = await currentUserService.GetCurrentUser();
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }
}