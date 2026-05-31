using System.Net;

using FluentAssertions;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class AuthControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly AppHttpClient _client = factory.CreateAppHttpClient();

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokensAndUserData()
    {
        // Arrange
        // The credentials below are pre-seeded in the test database by DbInitializer
        var request = new LoginRequest(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await _client.PostAsJsonAsync("/Auth/login", request);

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed with status {response.StatusCode}. Response: {content}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authDto = await response.Content.ReadFromJsonAsync<AuthDto>();
        authDto.Should().NotBeNull();
        authDto.Token.Should().NotBeNull();
        authDto.Token.AccessToken.Should().NotBeNullOrWhiteSpace();
        authDto.Token.RefreshToken.Should().NotBeNullOrWhiteSpace();
        authDto.User.Should().NotBeNull();
        authDto.User.Name.Should().Be("Admin User");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsErrorResponse()
    {
        // Arrange
        var request = new LoginRequest("nonexistent@quiznova.local", "WrongPassword123!", "Student");

        // Act
        var response = await _client.PostAsJsonAsync("/Auth/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefreshToken_WithMissingRefreshTokenCookie_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var request = new RefreshTokenRequest("dummy-access-token");

        // we didn't put refresh token in the cookies 
        // Act
        var response = await client.PostAsJsonAsync("/Auth/refresh", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithEmptyAccessToken_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var request = new RefreshTokenRequest(string.Empty);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh")
        {
            Content = JsonContent.Create(request),
        };
        httpRequest.Headers.Add("Cookie", "refreshToken=dummy-token");

        // Act
        var response = await client.SendAsync(httpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithMalformedAccessToken_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var request = new RefreshTokenRequest("gibberish-access-token");
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh")
        {
            Content = JsonContent.Create(request),
        };
        httpRequest.Headers.Add("Cookie", "refreshToken=dummy-token");

        // Act
        var response = await client.SendAsync(httpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidCookieNotInDb_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // 1. Login to get a valid access token
        var loginRequest = new LoginRequest(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var loginResponse = await client.PostAsJsonAsync("/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthDto>();
        var validAccessToken = loginResult!.Token.AccessToken;

        // 2. Refresh with fake cookie
        var request = new RefreshTokenRequest(validAccessToken);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh")
        {
            Content = JsonContent.Create(request),
        };
        httpRequest.Headers.Add("Cookie", "refreshToken=fake-token-not-in-db");

        // Act
        var response = await client.SendAsync(httpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RefreshToken_WithValidTokens_ReturnsNewTokens()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // 1. Login to get initial tokens
        var loginRequest = new LoginRequest(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var loginResponse = await client.PostAsJsonAsync("/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthDto>();
        var validAccessToken = loginResult!.Token.AccessToken;
        var validRefreshToken = loginResult.Token.RefreshToken;

        // 2. Perform refresh using manual cookie
        var refreshRequest = new RefreshTokenRequest(validAccessToken);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh")
        {
            Content = JsonContent.Create(refreshRequest),
        };
        httpRequest.Headers.Add("Cookie", $"refreshToken={validRefreshToken}");

        // Act
        var response = await client.SendAsync(httpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();
        tokenDto.Should().NotBeNull();
        tokenDto.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokenDto.RefreshToken.Should().NotBeNullOrWhiteSpace();
        tokenDto.RefreshToken.Should().NotBe(validRefreshToken);
    }

    [Fact]
    public async Task RefreshToken_ReusingRevokedRefreshToken_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // 1. Login to get initial tokens
        var loginRequest = new LoginRequest(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var loginResponse = await client.PostAsJsonAsync("/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthDto>();
        var originalAccessToken = loginResult!.Token.AccessToken;
        var originalRefreshToken = loginResult.Token.RefreshToken;

        // 2. First refresh (consumes/revokes the originalRefreshToken)
        var refreshRequest1 = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh")
        {
            Content = JsonContent.Create(new RefreshTokenRequest(originalAccessToken)),
        };
        refreshRequest1.Headers.Add("Cookie", $"refreshToken={originalRefreshToken}");
        var response1 = await client.SendAsync(refreshRequest1);
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Second refresh using the same originalRefreshToken (replay attack scenario)
        var refreshRequest2 = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh")
        {
            Content = JsonContent.Create(new RefreshTokenRequest(originalAccessToken)),
        };
        refreshRequest2.Headers.Add("Cookie", $"refreshToken={originalRefreshToken}");

        // Act
        var response2 = await client.SendAsync(refreshRequest2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
