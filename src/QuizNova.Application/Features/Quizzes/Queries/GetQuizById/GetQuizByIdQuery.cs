using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

public sealed record GetQuizByIdQuery(Guid QuizId)
    : ICachedQuery<Result<QuizDto>>
{
    public string CacheKey => $"quizzes:{QuizId}";

    public string[] Tags => ["quizzes"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
