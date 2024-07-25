using System.Security.Claims;
using Bonfire.Abstractions;
using Bonfire.Application.Services;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Tests;

public class ConversationsServiceTests
{
    public User CreateUser(long id = 1, string nick = "test")
    {
        var user = A.Fake<User>();
        user.Id = id;
        user.Nickname = nick;
        user.Conversations = new List<Conversation>();
        user.PasswordHash = "$2a$11$h7D6B.QDKZCSzlHfXa.hpO7bB9ySYwkRdI6VQTxl4sp0K/b6F61Fq";

        return user;
    }
    

    
    [Fact(DisplayName = "При создании переписки она должна иметь тип 'Диалог', если в ней всего 2 пользователя")]
    public async void Conversation_Should_Be_Created_As_Dialogue_If_Two_Users_In_It()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);

        var user = CreateUser();
        var user1 = CreateUser(2, "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var conversationsService = new ConversationsService(context, currentUserService);
        
        await context.AddRangeAsync(user, user1);
        await context.SaveChangesAsync();
        var participants = new List<long> { user1.Id };
        
        // Act
        var result = await conversationsService.CreateConversation(new ConversationRequest {UsersIds = participants});
        
        // Assert
        result.ConversationType.Should().Be(ConversationType.Dialogue);
        

    }
    
    [Fact(DisplayName = "При создании переписки она должна иметь тип 'Беседа', если в ней 3 и более пользователя")]
    public async void Conversation_Should_Be_Created_As_Group_Conversation_If_Three_Or_More_Users_In_It()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var currentUserService = A.Fake<ICurrentUserService>();
        
        var user = CreateUser();
        var user1 = CreateUser(2, "test1");
        var user2 = CreateUser(3, "test2");
        var user3 = CreateUser(4, "test3");
        
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        
        var conversationsService = new ConversationsService(context, currentUserService);
        
        await context.AddRangeAsync(user, user1, user2, user3);
        await context.SaveChangesAsync();
        var participants = new List<long> { user1.Id, user2.Id, user3.Id};
        
        // Act
        var result = await conversationsService.CreateConversation(new ConversationRequest {UsersIds = participants});
        
        // Assert
        result.ConversationType.Should().Be(ConversationType.Conversation);
        

    }
    
    [Fact(DisplayName = "При создании диалога, который уже существует, должна возвращаться информация о нём.")]
    public async void Dto_Should_Be_Returned_If_Dialogue_Already_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        var user1 = CreateUser(2, "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var conversationsService = new ConversationsService(context, currentUserService);
       
        var participantsIds = new List<long> { user1.Id };
        var receivers = await context.Users.Where(u => participantsIds.Contains(u.Id)).ToListAsync();
        var conversation = new Conversation(new List<Message>(), new List<User>(receivers){user}, ConversationType.Dialogue);
        
        await context.AddRangeAsync(user, user1);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = await conversationsService.CreateConversation(new ConversationRequest {UsersIds = participantsIds});
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.ConversationType.Should().Be(ConversationType.Dialogue);

    }
    
    [Fact(DisplayName = "При попытке создания диалога с самим собой, должна выдаваться ошибка")]
    public async void Conversation_Should_Not_Be_Created_If_Receiver_Equals_Sender()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        var user1 = CreateUser(2, "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var conversationsService = new ConversationsService(context, currentUserService);
       
        var participantsIds = new List<long> { user.Id, user1.Id };
        var receivers = await context.Users.Where(u => participantsIds.Contains(u.Id)).ToListAsync();
        var conversation = new Conversation(new List<Message>(), new List<User>(receivers){user}, ConversationType.Dialogue);
        
        await context.AddRangeAsync(user, user1);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result =  async() =>
        {
            await conversationsService.CreateConversation(new ConversationRequest
                { UsersIds = participantsIds });
        };
        
        // Assert
        await result.Should().ThrowAsync<ReceiverEqualsSenderException>();

    }
    
    [Fact(DisplayName = "При попытке создания переписки, где в списке пользователей несуществующий Id, должна выдаться ошибка")]
    public async void Conversation_Should_Not_Be_Created_If_Users_Ids_Is_Wrong()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        var user1 = CreateUser(2, "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var conversationsService = new ConversationsService(context, currentUserService);
       
        var participantsIds = new List<long> { 321, user1.Id };
        var receivers = await context.Users.Where(u => participantsIds.Contains(u.Id)).ToListAsync();
        var conversation = new Conversation(new List<Message>(), new List<User>(receivers){user}, ConversationType.Dialogue);
        
        await context.AddRangeAsync(user, user1);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result =  async() =>
        {
             await conversationsService.CreateConversation(new ConversationRequest
                { UsersIds = participantsIds });
        };
        
        // Assert
        await result.Should().ThrowAsync<WrongConversationParticipantsIdsException>();

    }
    
    [Fact(DisplayName = "При попытке выйти из переписки, которая не существует, должна выдаваться ошибка.")]
    public async void Conversation_Should_Not_Be_Removed_If_It_Is_Not_Existing()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var conversationsService = new ConversationsService(context, currentUserService);
        
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result =  async() =>
        {
            await conversationsService.ExitConversation(1);
        };
        
        // Assert
        await result.Should().ThrowAsync<ConversationNotFoundException>();

    }
    
    [Fact(DisplayName = "При попытке выйти из чужой переписки должна выдываться ошибка.")]
    public async void Conversation_Should_Not_Be_Removed_If_User_Does_Not_Own_it()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        var user1 = CreateUser(2, "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var conversationsService = new ConversationsService(context, currentUserService);
        var participantsIds = new List<long> { user1.Id };
        var receivers = await context.Users.Where(u => participantsIds.Contains(u.Id)).ToListAsync();
        var conversation = new Conversation(new List<Message>(), new List<User>(receivers), ConversationType.Conversation);
        
        await context.AddRangeAsync(user, user1);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();

        
        // Act
        var result =  async() =>
        {
            await conversationsService.ExitConversation(1);
        };
        
        // Assert
        await result.Should().ThrowAsync<AccessToConversationDeniedException>();

    }
}