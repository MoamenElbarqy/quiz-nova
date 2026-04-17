namespace QuizNova.Application.Features.Identity.Dtos;

public class TokenDto
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTime ExpiresOnUtc { get; set; }
}
