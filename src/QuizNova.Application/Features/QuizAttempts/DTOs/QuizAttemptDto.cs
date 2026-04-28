using QuizNova.Application.Features.Quizzes.DTOs;

namespace QuizNova.Application.Features.QuizAttempts.DTOs;

public sealed record QuizAttemptDto(
    Guid QuizAttemptId,
    Guid QuizId,
    string QuizTitle,
    DateTime StartedAt,
    DateTime SubmittedAt,
    int TotalQuestions,
    int AnsweredQuestions,
    int CorrectAnswers,
    int Score,
    IReadOnlyList<QuestionDto> Questions,
    IReadOnlyList<QuestionAnswerDto> Answers,
    bool IsPassed);
