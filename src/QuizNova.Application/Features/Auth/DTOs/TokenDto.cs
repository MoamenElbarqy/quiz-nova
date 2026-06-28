namespace QuizNova.Application.Features.Auth.DTOs;

public class TokenDto
{
    public string AccessToken { get; init; }

    public string RefreshToken { get; init; }

    public DateTime ExpiresOnUtc { get; init; }
}
