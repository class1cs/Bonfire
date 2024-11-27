
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

    [Fact(DisplayName = "Пользователь отправляет запрос на логин с корректными данными и получает токен. [200]")]
    public async Task Login_Should_Return_200_If_Successful()
    {
        // Arrange
        var credentials = new RegisterRequestDto("testUser", "testPassword");
        await _client.PostAsJsonAsync(Routes.Auth.Register, credentials);
        
        // Act
        var loginRequest = await _client.PostAsJsonAsync(Routes.Auth.Login, credentials);
        
        // Assert
        loginRequest.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на логин с данными для несуществующего аккаунта и получает ошибку. [400]")]
    public async Task Login_Should_Return_Error_If_Account_Does_Not_Exists()
    {
        // Arrange
        var credentials = new LoginRequestDto("testUser", "testPassword");
        
        // Act
        var loginRequest = await _client.PostAsJsonAsync(Routes.Auth.Login, credentials);
        
        // Assert
        loginRequest.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на логин с некорректными данными для аккаунта и получает ошибку. [400]")]
    public async Task Login_Should_Return_Error_If_Credentials_Are_Incorrect()
    {
        // Arrange
        var rightCredentials = new LoginRequestDto("testUser", "testPassword");
        var wrongCredentials = new RegisterRequestDto("testUser", "wrongPassword");
        await _client.PostAsJsonAsync(Routes.Auth.Register, rightCredentials);
        
        // Act
        var loginRequest = await _client.PostAsJsonAsync(Routes.Auth.Login, wrongCredentials);
        
        // Assert
        loginRequest.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на регистрацию с ником, не принадлежащим ни одному аккаунту. [200]")]
    public async Task Register_Should_Return_Token_If_Account_Does_Not_Exists()
    {
        // Arrange
        var registerData = new RegisterRequestDto("testUser", "testPassword");
        
        // Act
        var registerRequest = await _client.PostAsJsonAsync(Routes.Auth.Register, registerData);
        
        // Assert
        registerRequest.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на регистрацию с ником, уже принадлежащему чьему-то аккаунту. [409]")]
    public async Task Register_Should_Return_Error_If_Account_Already_Exists()
    {
        // Arrange
        var registerData = new RegisterRequestDto("testUser", "testPassword");
        await _client.PostAsJsonAsync(Routes.Auth.Register, registerData);
        
        // Act
        var registerRequest = await _client.PostAsJsonAsync(Routes.Auth.Register, registerData);
        
        // Assert
        registerRequest.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на регистрацию с пустыми полями. [400]")]
    public async Task Register_Should_Return_Error_If_Data_Is_Empty()
    {
        // Arrange
        var registerData = new RegisterRequestDto(string.Empty, string.Empty);

        // Act
        var registerRequest = await _client.PostAsJsonAsync(Routes.Auth.Register, registerData);

        // Assert
        registerRequest.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public async Task DisposeAsync() => await _application.ResetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;
}