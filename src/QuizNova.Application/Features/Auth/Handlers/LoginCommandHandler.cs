using System.Security.Claims;

using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.Commands;
using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Handlers;

public sealed class LoginCommandHandler(
    ILogger<LoginCommandHandler> logger,
    IAuthService authService)
    : IRequestHandler<LoginCommand, Result<AuthDto>>
{
    public async Task<Result<AuthDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var userResult = await authService.AuthenticateAsync(request.Email, request.Password);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var tokenResult = await authService.GenerateJwtTokenAsync(userResult.Value, ct);

        if (tokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", tokenResult.TopError.Description);
            return tokenResult.Errors;
        }

        var user = userResult.Value;
        var userName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? user.Name;
        var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? user.Role;

        return new AuthDto(
            tokenResult.Value,
            new UserDto(user.UserId, userName, userRole, user.Claims.ToList()));
    }
}
