
using System.Net;
using System.Net.Http.Json;
using Bonfire.API;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Tests.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace Bonfire.Tests;

public class AuthControllerTests : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
{
    private readonly ApiWebApplicationFactory _application;
    private readonly HttpClient _client; 
    
    
    public AuthControllerTests(ApiWebApplicationFactory application)
    {
        _application = application;
        _client = application.CreateClient();
    }

    [Fact(DisplayName = "Аккаунт существует, пользователь вводит правильные данные, получает токен. [200]")]
    public async Task Login_Should_Return_200_If_Successful()
    {
        // Arrange
        var credentials = new RegisterRequestDto("testUser", "testPassword");
        
        // Act
        await _client.PostAsJsonAsync(Routes.Auth.Register, credentials);
        var loginRequest = await _client.PostAsJsonAsync(Routes.Auth.Login, credentials);
        
        // Assert
        loginRequest.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact(DisplayName = "Аккаунт не существует -> логин -> ошибка [400]")]
    public async Task Login_Should_Return_Error_If_Account_Does_Not_Exists()
    {
        // Arrange
        var credentials = new LoginRequestDto("testUser", "testPassword");
        
        // Act
        var loginRequest = await _client.PostAsJsonAsync(Routes.Auth.Login, credentials);
        
        // Assert
        loginRequest.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public async Task DisposeAsync() => await _application.ResetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;
}