using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
{
    public Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        return authService.RefreshTokenAsync(request.ExpiredAccessToken, request.RefreshToken, ct);
    }
}
