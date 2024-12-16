using System.Net;
using System.Net.Http.Json;
using Bonfire.API;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Bonfire.Tests;

public class ChatControllerTests : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
{
    private readonly ApiWebApplicationFactory _application;

    private readonly HttpClient _client;

    public ChatControllerTests(ApiWebApplicationFactory application) => _application = application;

    public async Task DisposeAsync() => await _application.ResetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Пользователь отправляет запрос на получение переписок и получает их. [200]")]
    public async Task Conversation_Should_Be_Returned_As_List()
    {
        // Arrange
        var client = _application.WithWebHostBuilder(builder =>
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
            });

        var registerData = new RegisterRequestDto("firstUser", "123");
        await client.PostAsJsonAsync(Routes.Auth.Register, registerData);

        client.DefaultRequestHeaders.Authorization =
            new(scheme: "TestScheme");

        // Act
        var conversationsRequest = await client.GetAsync(Routes.Chat.GetConversations);

        // Assert
        conversationsRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на создание переписки с самим собой и получает ошибку. [400]")]
    public async Task Conversation_Should_Not_Be_Created_If_Receiver_Equals_Sender()
    {
        // Arrange
        var client = _application.WithWebHostBuilder(builder =>
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
            });

        var registerData = new RegisterRequestDto("firstUser", "123");
        await client.PostAsJsonAsync(Routes.Auth.Register, registerData);

        client.DefaultRequestHeaders.Authorization =
            new(scheme: "TestScheme");

        var conversationCreateRequest = new ConversationRequestDto([1]);

        // Act
        var conversationsRequest = await client.PostAsJsonAsync(Routes.Chat.CreateConversation, conversationCreateRequest);

        // Assert
        conversationsRequest.StatusCode.Should()
            .Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на создание переписки с несуществующим пользователем и получает ошибку. [400]")]
    public async void Conversation_Should_Not_Be_Created_If_Users_Ids_Is_Wrong()
    {
        // Arrange
        var client = _application.WithWebHostBuilder(builder =>
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
            });

        var registerData = new RegisterRequestDto("firstUser", "123");
        await client.PostAsJsonAsync(Routes.Auth.Register, registerData);

        client.DefaultRequestHeaders.Authorization =
            new(scheme: "TestScheme");

        var conversationCreateRequest = new ConversationRequestDto([2]);

        // Act
        var conversationsRequest = await client.PostAsJsonAsync(Routes.Chat.CreateConversation, conversationCreateRequest);

        // Assert
        conversationsRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
}