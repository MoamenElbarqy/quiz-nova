using System.Security.Claims;

using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.Commands;
using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Handlers;

public sealed class RefreshTokenCommandHandler(
    ILogger<RefreshTokenCommandHandler> logger,
    IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
{
    public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principal = authService.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);
        if (principal is null)
        {
            logger.LogError("Expired access token is not valid");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            logger.LogError("Invalid userId claim");
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var getUserResult = await authService.GetUserByIdAsync(userId);
        if (getUserResult.IsError)
        {
            logger.LogError("Get user by id error occurred: {ErrorDescription}", getUserResult.TopError.Description);
            return getUserResult.Errors;
        }

        var validateRefreshTokenResult = await authService.ValidateAndRevokeRefreshTokenAsync(userId, request.RefreshToken, ct);
        if (validateRefreshTokenResult.IsError)
        {
            return validateRefreshTokenResult.Errors;
        }

        var tokenResult = await authService.GenerateJwtTokenAsync(getUserResult.Value, ct);
        if (tokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", tokenResult.TopError.Description);
            return tokenResult.Errors;
        }

        return tokenResult.Value;
    }
}
