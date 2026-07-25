using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;

public sealed record GetStudentQuizzesQuery(Guid StudentId)
    : ICachedQuery<Result<StudentQuizzesDto>>
{
    public string CacheKey => $"quizzes:students:{StudentId}";

    public string[] Tags => [CacheTags.Quizzes, CacheTags.Students];

}
