using QuizNova.Domain.Common.Results;
using MediatR;

namespace QuizNova.Application.Features.Identity.Queries.GenerateTokens;

public record GenerateTokenQuery(
    string Email,
    string Password) : IRequest<Result<TokenResponse>>;