using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    ILogger<LoginCommandHandler> logger,
    IIdentityService identityService,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, Result<AuthDto>>
{
    public async Task<Result<AuthDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var userResult = await identityService.AuthenticateAsync(request.Email, request.Password);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var tokenResult = await tokenService.GenerateJwtTokenAsync(userResult.Value, ct);

        if (tokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", tokenResult.TopError.Description);
            return tokenResult.Errors;
        }

        return new AuthDto(
            tokenResult.Value,
            userResult.Value);
    }
}
