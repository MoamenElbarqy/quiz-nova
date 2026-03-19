using System.Security.Claims;

using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string? policyName);

    Task<Result<UserDto>> AuthenticateAsync(string email, string password);

    Task<Result<UserDto>> GetUserByIdAsync(string userId);

    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsExistedAndValid(string requestRefreshToken);
}
