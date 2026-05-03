using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;

public sealed record GetInstructorQuizzesCountQuery(Guid InstructorId)
    : ICachedQuery<Result<QuizzesCountDto>>
{
    public string CacheKey => $"quizzes:instructors:{InstructorId}:count";

    public string[] Tags => ["quizzes", "instructors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}

