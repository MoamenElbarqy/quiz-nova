using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

public sealed record GetStudentQuizAttemptsCountQuery(Guid StudentId)
    : ICachedQuery<Result<QuizAttemptsCountDto>>
{
    public string CacheKey => $"quiz_attempts:student:{StudentId}:count";

    public string[] Tags => ["quiz_attempts", "students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}

