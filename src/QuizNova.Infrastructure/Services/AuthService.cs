using System.Security.Claims;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private static readonly Error NotImplementedError = Error.Failure(
        code: "Auth.Service.NotImplemented",
        description: "Authentication service is not implemented yet.");

    public Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct = default)
    {
        _ = user;
        _ = ct;

        return Task.FromResult<Result<TokenDto>>(NotImplementedError);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        _ = token;

        return null;
    }

    public Task<bool> IsInRoleAsync(string userId, string role)
    {
        _ = userId;
        _ = role;

        return Task.FromResult(false);
    }

    public Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
        _ = userId;
        _ = policyName;

        return Task.FromResult(false);
    }

    public Task<Result<UserDto>> AuthenticateAsync(string email, string password)
    {
        _ = email;
        _ = password;

        return Task.FromResult<Result<UserDto>>(NotImplementedError);
    }

    public Task<Result<UserDto>> GetUserByIdAsync(string userId)
    {
        _ = userId;

        return Task.FromResult<Result<UserDto>>(NotImplementedError);
    }

    public Task<string?> GetUserNameAsync(string userId)
    {
        _ = userId;

        return Task.FromResult<string?>(null);
    }

    public Task<bool> IsExistedAndValid(string requestRefreshToken)
    {
        _ = requestRefreshToken;

        return Task.FromResult(false);
    }
}
