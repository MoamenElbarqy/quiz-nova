using System.Security.Claims;

using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    Task<Result<UserDto>> AuthenticateAsync(string email, string password);

    Task<Result<UserDto>> GetUserByIdAsync(string userId);

    Task<Result<Success>> ValidateAndRevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken ct);
}
