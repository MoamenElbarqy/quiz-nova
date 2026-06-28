using System.Net.Http.Headers;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Auth.DTOs;

namespace QuizNova.Api.IntegrationTests.Common;

public class AppHttpClient(HttpClient httpClient) : IDisposable
{
    private string? _token;

    public async Task AuthenticateAsync(string email, string password, string role)
    {
        var token = await GenerateTokenAsync(email, password, role);
        SetAuthToken(token);
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        ApplyAuthorizationHeader(request);
        return await httpClient.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default)
    {
        ApplyAuthorizationHeader(request);
        return await httpClient.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value,
        CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(value),
        };
        ApplyAuthorizationHeader(request);
        return await httpClient.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(value),
        };
        ApplyAuthorizationHeader(request);
        return await httpClient.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        ApplyAuthorizationHeader(request);
        return await httpClient.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PatchAsJsonAsync<T>(string requestUri, T value,
        CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
        {
            Content = JsonContent.Create(value),
        };
        ApplyAuthorizationHeader(request);
        return await httpClient.SendAsync(request, ct);
    }

    public void Dispose()
    {
        httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task<string> GenerateTokenAsync(string email, string password, string role)
    {
        var loginRequest = new LoginRequest(email, password, role);

        var response = await httpClient.PostAsJsonAsync("/Auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Token generation failed with HTTP status {response.StatusCode}");
        }

        var authDto = await response.Content.ReadFromJsonAsync<AuthDto>();
        if (authDto?.Token.AccessToken is null)
        {
            throw new InvalidOperationException("Response did not contain a valid Access Token.");
        }

        return authDto.Token.AccessToken;
    }

    private void SetAuthToken(string token)
    {
        _token = token;
    }

    private void ApplyAuthorizationHeader(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }
}
