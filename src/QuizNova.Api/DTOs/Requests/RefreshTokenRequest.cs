namespace QuizNova.Api.DTOs.Requests;

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;

    public string ExpiredAccessToken { get; init; } = string.Empty;
}
