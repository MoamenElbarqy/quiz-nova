namespace QuizNova.Application.Features.QuizAttempts.DTOs;

public sealed record PendingManualAnswersDto(
    Guid AttemptId,
    Guid StudentId,
    string StudentName,
    string CourseName,
    string QuizTitle,
    DateTime SubmittedAt,
    int UngradedCount);
