using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;

public sealed record GetInstructorQuizzesQuery(Guid InstructorId)
    : ICachedQuery<Result<List<QuizDto>>>
{
    public string CacheKey => $"quizzes:instructor:{InstructorId}";

    public string[] Tags => [CacheTags.Quizzes, CacheTags.Instructors];

}
