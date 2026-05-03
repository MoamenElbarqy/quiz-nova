using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

public sealed record GetStudentQuizAttemptsQuery(Guid StudentId)
    : ICachedQuery<Result<List<QuizAttemptDto>>>
{
    public string CacheKey => $"quiz_attempts:student:{StudentId}";

    public string[] Tags => ["quiz_attempts", "students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
