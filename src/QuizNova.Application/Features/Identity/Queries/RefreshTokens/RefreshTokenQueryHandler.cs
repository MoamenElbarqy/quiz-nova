using System.Security.Claims;

using MediatR;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Identity.Queries.RefreshTokens;

public class RefreshTokenQueryHandler(
    IAuthService authService)
    : IRequestHandler<RefreshTokenQuery, Result<TokenDto>>
{
    public async Task<Result<TokenDto>> Handle(RefreshTokenQuery request, CancellationToken ct)
    {
        var principal = authService.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);
        if (principal is null)
        {
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var getUserResult = await authService.GetUserByIdAsync(userId);

        if (getUserResult.IsError)
        {
            return getUserResult.Errors;
        }

        var validateRefreshTokenResult = await authService.ValidateAndRevokeRefreshTokenAsync(userId, request.RefreshToken, ct);
        if (validateRefreshTokenResult.IsError)
        {
            return validateRefreshTokenResult.Errors;
        }

        var generateTokenResult = await authService.GenerateJwtTokenAsync(getUserResult.Value, ct);

        if (generateTokenResult.IsError)
        {
            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}
