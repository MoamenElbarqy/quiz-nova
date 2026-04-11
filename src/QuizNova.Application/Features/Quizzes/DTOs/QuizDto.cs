namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed record QuizDto(
    Guid QuizId,
    string Title,
    string CourseName,
    string InstructorName,
    int Marks,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    string State);
