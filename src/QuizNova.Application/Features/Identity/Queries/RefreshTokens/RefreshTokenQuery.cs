using MediatR;

using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Identity.Queries.RefreshTokens;

public record RefreshTokenQuery(
    string RefreshToken,
    string ExpiredAccessToken) : IRequest<Result<TokenDto>>;