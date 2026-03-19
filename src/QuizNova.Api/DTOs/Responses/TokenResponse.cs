namespace QuizNova.Api.DTOs.Responses;

public sealed class TokenResponse
{
    public string? AccessToken { get; init; }

    public string? RefreshToken { get; init; }

    public DateTime ExpiresOnUtc { get; init; }
}
