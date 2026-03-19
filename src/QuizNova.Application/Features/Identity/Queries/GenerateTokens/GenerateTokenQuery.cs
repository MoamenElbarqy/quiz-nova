using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Identity.Queries.GenerateTokens;

public record GenerateTokenQuery(
    string Email,
    string Password) : IRequest<Result<TokenDto>>;