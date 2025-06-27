using System.Net;
using System.Net.Http.Json;
using Bonfire.API;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Tests.Extensions;
using FluentAssertions;

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

    public async Task DisposeAsync() => await _application.ResetDatabase();

    public Task InitializeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Пользователь отправляет запрос на логин с корректными данными и получает токен. [200]")]
    public async Task Login_Should_Return_200_If_Successful()
    {
        // Arrange
        var credentials = new RegisterRequest("testUser", "testPassword");
        await _client.PostAsJsonAsync("api/auth/register", credentials);

        // Act
        var loginRequest = await _client.PostAsJsonAsync("api/auth/login", credentials);

        // Assert
        loginRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на логин с данными для несуществующего аккаунта и получает ошибку. [400]")]
    public async Task Login_Should_Return_Error_If_Account_Does_Not_Exists()
    {
        // Arrange
        var credentials = new LoginRequest("testUser", "testPassword");

        // Act
        var loginRequest = await _client.PostAsJsonAsync("api/auth/login", credentials);

        // Assert
        loginRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на логин с некорректными данными для аккаунта и получает ошибку. [400]")]
    public async Task Login_Should_Return_Error_If_Credentials_Are_Incorrect()
    {
        // Arrange
        var rightCredentials = new LoginRequest("testUser", "testPassword");
        var wrongCredentials = new RegisterRequest("testUser", "wrongPassword");
        await _client.PostAsJsonAsync("api/auth/register", rightCredentials);

        // Act
        var loginRequest = await _client.PostAsJsonAsync("api/auth/login", wrongCredentials);

        // Assert
        loginRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на регистрацию с ником, не принадлежащим ни одному аккаунту. [200]")]
    public async Task Register_Should_Return_Token_If_Account_Does_Not_Exists()
    {
        // Arrange
        var registerData = new RegisterRequest("testUser", "testPassword");

        // Act
        var registerRequest = await _client.PostAsJsonAsync("api/auth/register", registerData);

        // Assert
        registerRequest.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на регистрацию с ником, уже принадлежащему чьему-то аккаунту. [409]")]
    public async Task Register_Should_Return_Error_If_Account_Already_Exists()
    {
        // Arrange
        var registerData = new RegisterRequest("testUser", "testPassword");
        await _client.PostAsJsonAsync("api/auth/register", registerData);

        // Act
        var registerRequest = await _client.PostAsJsonAsync("api/auth/register", registerData);

        // Assert
        registerRequest.StatusCode.Should()
            .Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "Пользователь отправляет запрос на регистрацию с пустыми полями и получает ошибку. [400]")]
    public async Task Register_Should_Return_Error_If_Data_Is_Empty()
    {
        // Arrange
        var registerData = new RegisterRequest(string.Empty, string.Empty);

        // Act
        var registerRequest = await _client.PostAsJsonAsync("api/auth/register", registerData);

        // Assert
        registerRequest.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
}