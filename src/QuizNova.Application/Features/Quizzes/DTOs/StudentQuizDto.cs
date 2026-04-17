using QuizNova.Domain.Entities.Quizzes.Enums;

namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed record StudentQuizDto(
    Guid QuizId,
    string Title,
    string CourseName,
    int QuestionsCount,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    QuizStatus QuizStatus);
