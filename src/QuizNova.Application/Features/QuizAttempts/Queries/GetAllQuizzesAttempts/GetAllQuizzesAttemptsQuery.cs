using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public sealed record GetAllQuizzesAttemptsQuery(
    string? SearchTerm = null,
    int? CorrectAnswers = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<QuizAttemptDto>>>
{
    public string CacheKey => $"quiz_attempts:all:{SearchTerm}:{CorrectAnswers}:{PageNumber}:{PageSize}";

    public string[] Tags => [CacheTags.QuizAttempts];

}
