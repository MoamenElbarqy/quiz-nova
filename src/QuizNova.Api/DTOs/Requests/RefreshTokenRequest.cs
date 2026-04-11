namespace QuizNova.Api.DTOs.Requests;

public sealed record RefreshTokenRequest(
    string refreshToken,
    string expiredAccessToken);
