﻿using System.Security.Claims;
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

public class MessagesServiceTests
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
    
    [Fact(DisplayName = "При отправке сообщения в переписку должно возвращать DTO сообщения.")]
    public async void Message_Should_Be_Sent_And_Return_Dto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var user = CreateUser();
      
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message>();
        var participants = new List<User> { user };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(user);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = await messagesService.SendMessage(new MessageRequest {Text = "test"}, conversation.Id);
        
        // Assert
        result.Should().NotBeNull();
        
    }
    
    [Fact(DisplayName = "При попытке отправки пустого сообщения в переписку должна выдаваться ошибка.")]
    public async void  Message_Should_Not_Be_Sent_If_Text_Is_Empty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var user = CreateUser();
      
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message>();
        var participants = new List<User> { user };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(user);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async () =>
        {
             await messagesService.SendMessage(new MessageRequest { Text = string.Empty }, conversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<EmptyMessageTextException>();
        
    }
    
    [Fact(DisplayName = "При попытке отправки сообщения в несуществующую переписку должна выдаться ошибка.")]
    public async void  Message_Should_Not_Be_Sent_If_Conversation_Does_Not_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var messagesService = new MessagesService(context, currentUserService);
        
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.SendMessage(new MessageRequest{ Text = "test" }, 3);
        };
        
        // Assert
        await result.Should().ThrowAsync<ConversationNotFoundException>();

    }
    
    [Fact(DisplayName = "При попытке редактирования своего несуществующего сообщения должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Edited_If_It_Does_Not_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var user = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message>();
        var participants = new List<User> { user };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(user);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.EditMessage(new MessageRequest{ Text = "test" }, 123, conversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<MessageNotFoundException>();
    }
    
    [Fact(DisplayName = "При попытке редактирования своего сообщения не в той переписке должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Edited_If_Conversation_Is_Wrong()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var user = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message>{ new("test", DateTime.Now, user) };
        var participants = new List<User> { user };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        var secondConversation = new Conversation(new List<Message>(), participants, ConversationType.Conversation);
        
        await context.AddAsync(user);
        await context.AddRangeAsync(conversation, secondConversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.EditMessage(new MessageRequest{ Text = "test" }, 1, secondConversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<MessageNotFoundException>();
    }
    
    [Fact(DisplayName = "При попытке редактирования чужого сообщения должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Edited_If_User_Does_Not_Own_Message()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var firstUser = CreateUser();
        var secondUser = CreateUser(id: 2, name: "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message> { new("test", DateTime.Now, secondUser) };
        var participants = new List<User> { secondUser, firstUser };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(conversation);
        await context.AddAsync(firstUser);
        await context.AddAsync(secondUser);
        await context.SaveChangesAsync();
        // Act
        var result = async() =>
        {
            await messagesService.EditMessage(new MessageRequest{ Text = "test" }, conversation.Id, messages.FirstOrDefault()!.Id);
        };
        // Assert
        await result.Should().ThrowAsync<AccessToMessageDeniedException>();

    }
    
    [Fact(DisplayName = "При попытке удаления своего несуществующего сообщения должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Removed_If_It_Does_Not_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var user = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message>();
        var participants = new List<User> { user };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(user);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.RemoveMessage(123, conversation.Id);
        };
        // Assert
        await result.Should().ThrowAsync<MessageNotFoundException>();
    }
    
    [Fact(DisplayName = "При попытке удаления своего сообщения не в той переписке должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Removed_If_Conversation_Is_Wrong()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        var user = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(user);
        
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message>{ new("test", DateTime.Now, user) };
        var participants = new List<User> { user };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        var secondConversation = new Conversation(new List<Message>(), participants, ConversationType.Conversation);
        
        await context.AddAsync(user);
        await context.AddRangeAsync(conversation, secondConversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.RemoveMessage(1, secondConversation.Id);
        };
        // Assert
        await result.Should().ThrowAsync<MessageNotFoundException>();
    }
    
    [Fact(DisplayName = "При попытке удаления чужого сообщения должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Removed_If_User_Does_Not_Own_Message()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
      
        var firstUser = CreateUser();
        var secondUser = CreateUser(id: 2, name: "test1");
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message> { new("test", DateTime.Now, secondUser) };
        var participants = new List<User> { secondUser, firstUser };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(firstUser);
        await context.AddAsync(secondUser);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.RemoveMessage(1, conversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<AccessToMessageDeniedException>();

    }
    
    [Fact(DisplayName = "При попытке получения чужих сообщений должна выдаваться ошибка.")]
    public async void Messages_Should_Not_Be_Given_If_User_Does_Not_Involve_In_Conversation()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var firstUser = CreateUser();
        var secondUser = CreateUser("test1", 2);
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message> { new("test", DateTime.Now, secondUser) };
        var participants = new List<User> { secondUser };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(firstUser);
        await context.AddAsync(secondUser);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.GetMessages(conversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<AccessToConversationDeniedException>();
    }
    
    [Fact(DisplayName = "При попытке получения сообщений при несуществующей переписке должна выдываться ошибка.")]
    public async void Messages_Should_Not_Be_Given_If_Conversation_Does_Not_Exist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var firstUser = CreateUser();
        var secondUser = CreateUser("test1", 2);
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        var messagesService = new MessagesService(context, currentUserService);
        
        await context.AddAsync(firstUser);
        await context.AddAsync(secondUser);
        
        var messages = new List<Message> { new("test", DateTime.Now, secondUser) };
        var participants = new List<User> { secondUser };
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
            await messagesService.GetMessages(conversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<AccessToConversationDeniedException>();
    }
    
    [Fact(DisplayName = "При редактировании своего сообщения должно возвращать DTO сообщения.")]
    public async void Message_Should_Be_Edited_And_Return_Dto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var firstUser = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message> {new("test", DateTime.Now, firstUser)};
        var participants = new List<User>{firstUser};
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(firstUser);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = await messagesService.EditMessage(new MessageRequest{ Text = "tests" },1, conversation.Id);
        
        // Assert
        result.Should().BeOfType<MessageResponse>();
        result.Text.Should().Be("tests");
    }
    
    [Fact(DisplayName = "При попытке редактирования текста сообщения на пустоту должна выдаваться ошибка.")]
    public async void Message_Should_Not_Be_Edited_If_Message_Text_Is_Empty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var firstUser = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message> {new("test", DateTime.Now, firstUser)};
        var participants = new List<User>{firstUser};
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(firstUser);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = async() =>
        {
             await messagesService.EditMessage(new MessageRequest { Text = string.Empty }, 1, conversation.Id);
        };
        
        // Assert
        await result.Should().ThrowAsync<EmptyMessageTextException>();
    }
    
    [Fact(DisplayName = "При удалении своего сообщения должно возвращать DTO сообщения.")]
    public async void Message_Should_Be_Removed_And_Return_Dto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new AppDbContext(options);
        
        var firstUser = CreateUser();
        
        var currentUserService = A.Fake<ICurrentUserService>();
        A.CallTo(() => currentUserService.GetCurrentUser()).Returns(firstUser);
        var messagesService = new MessagesService(context, currentUserService);
        
        var messages = new List<Message> {new("test", DateTime.Now, firstUser)};
        var participants = new List<User>{firstUser};
        var conversation = new Conversation(messages, participants, ConversationType.Dialogue);
        
        await context.AddAsync(firstUser);
        await context.AddAsync(conversation);
        await context.SaveChangesAsync();
        
        // Act
        var result = await messagesService.RemoveMessage(conversation.Id, 1);
        
        // Assert
        result.Should().BeOfType<MessageResponse>().And.NotBeNull();
    }
}