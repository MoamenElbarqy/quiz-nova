namespace QuizNova.Application.Features.QuizAttempts.DTOs;

public sealed record QuizAttemptDto(
    Guid QuizAttemptId,
    Guid QuizId,
    string QuizTitle,
    DateTime StartedAt,
    DateTime? SubmittedAt,
    int TotalQuestions,
    int AnsweredQuestions,
    int CorrectAnswers,
    int Score,
    IReadOnlyList<StudentAttemptAnswerDto> Answers,
    bool IsSubmitted,
    bool IsPassed);
