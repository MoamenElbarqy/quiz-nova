using MediatR;

using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthDto>>;
