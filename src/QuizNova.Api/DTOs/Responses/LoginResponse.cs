namespace QuizNova.Api.DTOs.Responses;

public class LoginResponse
{
    public string Message { get; init; } = string.Empty;

    public TokenResponse? Token { get; init; }

    public UserResponse? User { get; init; }

}