using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;

public sealed record GetQuizAttemptByIdQuery(Guid QuizAttemptId) : ICachedQuery<Result<QuizAttemptDto>>
{
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);

    public string CacheKey => $"quiz_attempts:{QuizAttemptId}";

    public string[] Tags => ["quiz_attempts"];
}
