using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, Result<AuthDto>>
{
    public Task<Result<AuthDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        return authService.LoginAsync(request.Email, request.Password, request.Role, ct);
    }
}
