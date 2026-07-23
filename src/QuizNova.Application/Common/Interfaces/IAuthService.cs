using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<AuthDto>> LoginAsync(string email, string password, string requestedRole, CancellationToken ct);

    Task<Result<TokenDto>> RefreshTokenAsync(string expiredAccessToken, string refreshToken, CancellationToken ct);

    Task<Result<string>> RegisterUserAsync(string email, string password, string role);

    Task<string> GetUserNameAsync(string userId);
}
