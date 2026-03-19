using MediatR;

using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Commands;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result<AuthDto>>;
