using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Identity.Queries.GenerateTokens;

public class GenerateTokenQueryHandler(
    ILogger<GenerateTokenQueryHandler> logger,
    IAuthService authService)
    : IRequestHandler<GenerateTokenQuery, Result<TokenDto>>
{
    public async Task<Result<TokenDto>> Handle(GenerateTokenQuery query, CancellationToken ct)
    {
        var userResponse = await authService.AuthenticateAsync(query.Email, query.Password);

        if (userResponse.IsError)
        {
            return userResponse.Errors;
        }

        var generateTokenResult = await authService.GenerateJwtTokenAsync(userResponse.Value, ct);

        if (generateTokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}