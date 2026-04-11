using MediatR;

using QuizNova.Application.Features.Identity;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken,
                                         string ExpiredAccessToken) : IRequest<Result<TokenDto>>;
