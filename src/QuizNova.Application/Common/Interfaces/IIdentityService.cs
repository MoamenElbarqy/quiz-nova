using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserDto>> AuthenticateAsync(string email, string password);

    Task<Result<string>> RegisterUserAsync(string email, string password, string name, string role, CancellationToken ct);

    Task<Result<UserDto>> GetUserByIdAsync(string userId);

    Task<string> GetUserNameAsync(string userId);

    Task<Result<Success>> ValidateAndRevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken ct);
}
