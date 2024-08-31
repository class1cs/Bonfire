using System.Security.Claims;
using Bonfire.Application.Interfaces;
using Bonfire.Application.Services;
using Bonfire.Domain.Entities;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class UserInfoServiceTests
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

    [Fact(DisplayName = "Должно возвращать текущего пользователя, если  токен верен.")]
    public async void User_Should_Be_Returned_If_Token_Is_Right()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

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


        var userInfoService = new UserService(httpContextAccessor, context);

        // Act
        var result = await userInfoService.GetCurrentUser();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact(DisplayName = "Должно возвращать пользователей по имени при поиске.")]
    public async void Should_Return_User_If_Data_Correct()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var user = CreateUser();
        var user1 = CreateUser("test1", 2);
        var httpContextAccessor = A.Fake<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = A.Fake<HttpContext>();
        httpContextAccessor.HttpContext.User = A.Fake<ClaimsPrincipal>();
        var currentUserService = A.Fake<IUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var userIdClaim = A.Fake<Claim>(x => x.WithArgumentsForConstructor(() => new Claim("Id", "1")));
        A.CallTo(() => httpContextAccessor.HttpContext.User.Identity!.IsAuthenticated).Returns(true);
        A.CallTo(() => httpContextAccessor.HttpContext.User.Claims).Returns(new List<Claim> { userIdClaim });

        await context.AddRangeAsync(user, user1);
        await context.SaveChangesAsync();
        var userInfoService = new UserInfoService(currentUserService, context);

        // Act
        var result = await userInfoService.SearchUser("test");

        // Assert
        result.Length.Should().Be(1);
        result[0].NickName.Should().Be("test1");
    }

    [Fact(DisplayName = "Должно возвращать пустой список при поиске, если пользователи, кроме текущего, не найдены.")]
    public async void Should_Return_Blank_List_If_User_Except_Current_Not_Found()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var user = CreateUser();
        var httpContextAccessor = A.Fake<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = A.Fake<HttpContext>();
        httpContextAccessor.HttpContext.User = A.Fake<ClaimsPrincipal>();
        var currentUserService = A.Fake<IUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        await context.AddAsync(user);
        await context.SaveChangesAsync();

        var userInfoService = new UserInfoService(currentUserService, context);

        // Act
        var result = await userInfoService.SearchUser("test");

        // Assert
        result.Length.Should().Be(0);
    }
}