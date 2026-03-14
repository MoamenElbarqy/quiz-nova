using QuizNova.Domain.Common.Results;

using MediatR;

namespace QuizNova.Application.Features.Identity.Queries.RefreshTokens;

public record RefreshTokenQuery(
    string RefreshToken,
    string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;