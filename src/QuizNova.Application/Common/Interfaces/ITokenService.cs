using System.Security.Claims;

using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Common.Interfaces;

public interface ITokenService
{
    Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
