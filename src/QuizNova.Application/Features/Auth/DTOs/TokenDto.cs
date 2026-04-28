namespace QuizNova.Application.Features.Auth.DTOs;

public class TokenDto
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTime ExpiresOnUtc { get; set; }
}
