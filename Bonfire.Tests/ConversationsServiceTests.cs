using System.Security.Claims;
using AutoMapper;
using Bonfire.Application.Services;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class ConversationsServiceTests
{
    public User CreateUser()
    {
        var user = A.Fake<User>();
        user.Id = 2;
        user.Nickname = "test";
        user.Conversations = new List<Conversation>();
        user.PasswordHash = "$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq";

        return user;
    }
    private readonly IMapper _mapper;
    public ConversationsServiceTests()
    {
        var config = new MapperConfiguration(x =>
        {
            x.CreateMap<Message, MessageResponse>().IncludeAllDerived();
        
            x.CreateMap<User, UserResponse>();
        
            x.CreateMap<Conversation, MessagesResponse>();
        });
        _mapper = config.CreateMapper();
    }

    
    [Fact(DisplayName = "При создании переписки она должна иметь тип 'Диалог', если в ней всего 2 пользователя")]
    public async void Conversation_Should_Be_Created_As_Dialogue_If_Two_Users_In_It()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var httpContextAccessor = A.Fake<IHttpContextAccessor>();
        var claims = new List<Claim>()
        {
            new("Id", 1.ToString())
        };
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var currentUserService = A.Fake<CurrentUserService>(x => x.WithArgumentsForConstructor([httpContextAccessor, context]));
        var conversationsService = A.Fake<ConversationsService>(x => x.WithArgumentsForConstructor([ context, _mapper, currentUserService]));
        
      
        
        var user = CreateUser();
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        var participants = new List<long> { user.Id };
        
        // Act
        var result = await conversationsService.CreateConversation(new ConversationRequest(participants));
        
        // Assert
        result.Should().BeOfType<ConversationResponse>();

    }
}