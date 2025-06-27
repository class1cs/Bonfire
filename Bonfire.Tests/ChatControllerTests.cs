using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bonfire.API;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bonfire.Tests;

public class ChatControllerTests : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
{
    private readonly ApiWebApplicationFactory _application;

    private readonly HttpClient _client;

    public ChatControllerTests(ApiWebApplicationFactory application)
    {
        _application = application;
        _client = _application.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options =>
                    {
                    });
            });
        })
        .CreateClient(new()
        {
            AllowAutoRedirect = false
        });;
    }

    public async Task DisposeAsync() => await _application.ResetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Пользователь отправляет запрос на получение переписок и получает их. [200]")]
    public async Task Conversation_Should_Be_Returned_As_List()
    {
        // Arrange
        var registerData = new RegisterRequest("firstUser", "123");
        await _client.PostAsJsonAsync("api/auth/register", registerData);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var conversationRequest = await _client.GetAsync("api/chat/conversations");

        // Assert
        conversationRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на создание переписки с самим собой и получает ошибку. [400]")]
    public async Task Conversation_Should_Not_Be_Created_If_Receiver_Equals_Sender()
    {
        // Arrange
        var registerData = new RegisterRequest("firstUser", "123");
        await _client.PostAsJsonAsync("api/auth/register", registerData);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");

        var conversationCreateRequest = new ConversationRequest([1]);

        // Act
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);

        // Assert
        conversationRequest.StatusCode.Should()
            .Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на создание переписки с несуществующим пользователем и получает ошибку. [400]")]
    public async void Conversation_Should_Not_Be_Created_If_Users_Ids_Is_Wrong()
    {
        // Arrange
        var registerData = new RegisterRequest("firstUser", "123");
        await _client.PostAsJsonAsync("api/auth/register", registerData);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");

        var conversationCreateRequest = new ConversationRequest([2]);

        // Act
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);

        // Assert
        conversationRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на создание уже существующего диалога и получает этот же диалог. [200]")]
    public async void Conversation_Should_Be_Returned_If_It_Already_Exists()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);

        // Act
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);

        // Assert
        conversationRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на создание переписки с одним пользователем и она создаётся с типом диалога. [200]")]
    public async void Conversation_Should_Be_Created_As_Dialogue_If_Two_Users_In_It()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);

        // Act
        var conversationRequest = await _client.PostAsJsonAsync("/api/chat/conversations", conversationCreateRequest);
        var json = await conversationRequest.Content.ReadAsStringAsync();
        
        var response = JsonSerializer.Deserialize<ConversationResponse>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        // Assert
        conversationRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        response.ConversationType.Should()
            .Be(ConversationType.Dialogue);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на создание переписки с двумя или более пользователями и она создаётся с типом беседы. [200]")]
    public async void Conversation_Should_Be_Created_As_Group_Conversation_If_Three_Or_More_Users_In_It()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2, 3]);

        // Act
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        var json = await conversationRequest.Content.ReadAsStringAsync();
        
        var response = JsonSerializer.Deserialize<ConversationResponse>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        // Assert
        conversationRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        response.ConversationType.Should()
            .Be(ConversationType.Conversation);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на отправку сообщения и получает его DTO. [200]")]
    public async void Message_Should_Be_Sent_And_Return_Dto()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
         
        // Act
        var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test"));
        
        // Assert
        sendMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на получение 50 сообщений и получает их в порядке отправки [200].")]
    public async Task GetMessages_ShouldReturn50MessagesInOrder_WhenRequested()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
    
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
    
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
    
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();

        // Отправка 50 сообщений
        for (int i = 0; i < 50; i++)
        {
            var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest($"test message {i + 1}"));
            sendMessageRequest.EnsureSuccessStatusCode();
        }
    
        // Act
        var getMessagesRequest = await _client.GetAsync($"api/chat/conversations/1/messages");
    
        // Assert
        getMessagesRequest.StatusCode.Should().Be(HttpStatusCode.OK);

        var messagesResponse = await getMessagesRequest.Content.ReadFromJsonAsync<MessagesResponse>();
    
        messagesResponse.Messages.Should().HaveCount(50);
    }

    
    [Fact(DisplayName = "Пользователь отправляет запрос на получение чужих сообщений и получает ошибку [403].")]
    public async void Messages_Should_Not_Be_Given_If_Conversation_Does_Not_Belong_To_User()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var sendMessageRequest = await _client.GetAsync($"api/chat/conversations/1/messages");
        
        // Assert
        sendMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.Forbidden);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на отправку пустого сообщения и получает ошибку. [400]")]
    public async void Message_Should_Not_Be_Sent_If_Text_Is_Empty()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
         
        // Act
        var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest(string.Empty));
        
        // Assert
        sendMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на редактирование сообщения на пустой текст и получает ошибку. [400]")]
    public async void Message_Should_Not_Be_Edited_If_Text_Is_Empty()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
                                                                    
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test"));
         
        // Act
        var editMessageRequest =  await _client.PutAsJsonAsync($"api/chat/conversations/1/messages/1", new MessageRequest(String.Empty));
        
        // Assert
        editMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на редактирование несуществующего сообщения и получает ошибку. [404]")]
    public async void Message_Should_Not_Be_Edited_If_Message_Does_Not_Exist()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        // Act
        var editMessageRequest =  await _client.PutAsJsonAsync($"api/chat/conversations/1/messages/1", new MessageRequest("test"));
        
        // Assert
        editMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на редактирование чужого сообщения и получает ошибку. [403]")]
    public async void Message_Should_Not_Be_Edited_If_Message_Does_Not_Belong_To_Him()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test1"));
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var editMessageRequest = await _client.PutAsJsonAsync($"api/chat/conversations/1/messages/1", new MessageRequest("test"));
        
        // Assert
        editMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.Forbidden);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на редактирование своего сообщения не в той переписке и получает ошибку. [404]")]
    public async void Message_Should_Not_Be_Edited_If_Message_Belong_To_Wrong_Conversation()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test1"));
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var editMessageRequest = await _client.PutAsJsonAsync($"api/chat/conversations/2/messages/1", new MessageRequest("test"));
        
        // Assert
        editMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на отправку сообщения, состоящего из одних пробелов, и получает ошибку. [400]")]
    public async void Message_Should_Not_Be_Sent_If_Text_Is_WhiteSpaced()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
         
        // Act
        var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("        "));
        // Assert
        sendMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на получение сообщений в несуществующей переписке и получает ошибку. [404]")]
    public async void Messages_Should_Not_Be_Given_If_Conversation_Does_Not_Exist()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test"));
        sendMessageRequest.EnsureSuccessStatusCode();
        
        // Act
        var getMessagesRequest = await _client.GetAsync($"api/chat/conversation/2/messages");
        // Assert
        getMessagesRequest.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
        
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на отправку сообщения в несуществующую переписку и получает ошибку. [404]")]
    public async void Messages_Should_Not_Be_Sent_If_Conversation_Does_Not_Exist()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        var conversationCreateRequest = new ConversationRequest([2]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        // Act
        var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/2/messages", new MessageRequest("Test"));
        
        // Assert
        sendMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на отправку сообщения в чужую переписку и получает ошибку. [404]")]
    public async void Messages_Should_Not_Be_Sent_If_It_Sent_In_Conversation_Where_User_Is_Not_In()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var sendMessageRequest = await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("Test"));
        
        // Assert
        sendMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.Forbidden);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на удаление чужого сообщения и получает ошибку. [403]")]
    public async void Message_Should_Not_Be_Deleted_If_Message_Does_Not_Belong_To_Him()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test1"));
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var deleteMessageRequest = await _client.DeleteAsync($"api/chat/conversations/1/messages/1");
        
        // Assert
        deleteMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.Forbidden);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на удаление несуществующего сообщения и получает ошибку. [404]")]
    public async void Message_Should_Not_Be_Deleted_If_Message_Does_Not_Exist()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        // Act
        var deleteMessageRequest = await _client.DeleteAsync($"api/chat/conversations/1/messages/1");
        
        // Assert
        deleteMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на удаление своего сообщения и получает его DTO. [200]")]
    public async void Message_Should_Be_Deleted()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test1"));
        
        // Act
        var deleteMessageRequest = await _client.DeleteAsync($"api/chat/conversations/1/messages/1");
        
        // Assert
        deleteMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }
    
    [Fact(DisplayName = "Пользователь отправляет запрос на удаление своего сообщения не в той переписке и получает ошибку. [404]")]
    public async void Message_Should_Not_Be_Deleted_If_Message_Belong_To_Wrong_Conversation()
    {
        // Arrange
        var firstUserRegisterData = new RegisterRequest("firstUser", "123");
        var secondUserRegisterData = new RegisterRequest("secondUser", "123");
        var thirdUserRegisterData = new RegisterRequest("thirdUser", "123");
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("2");
        
        await _client.PostAsJsonAsync("api/auth/register", firstUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", secondUserRegisterData);
        await _client.PostAsJsonAsync("api/auth/register", thirdUserRegisterData);
        
        var conversationCreateRequest = new ConversationRequest([3]);
        var conversationRequest = await _client.PostAsJsonAsync("api/chat/conversations", conversationCreateRequest);
        conversationRequest.EnsureSuccessStatusCode();
        
        await _client.PostAsJsonAsync($"api/chat/conversations/1/messages", new MessageRequest("test1"));
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("1");
        
        // Act
        var deleteMessageRequest = await _client.DeleteAsync($"api/chat/conversations/2/messages/1");
        
        // Assert
        deleteMessageRequest.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
}