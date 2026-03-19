using MediatR;

using QuizNova.Application.Features.Auth.Commands;
using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Handlers;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthDto>>
{
    public Task<Result<AuthDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        _ = ct;

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return Task.FromResult<Result<AuthDto>>(
                Error.Validation("Auth.Register.PasswordMismatch", "Password and confirm password do not match."));
        }

        return Task.FromResult<Result<AuthDto>>(
            Error.Failure("Auth.Register.NotImplemented", "Register flow is not implemented yet."));
    }
}
