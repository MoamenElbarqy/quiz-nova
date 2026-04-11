namespace QuizNova.Application.Features.QuizAttempts.DTOs;

public sealed record StudentAttemptAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool? IsCorrect,
    Guid? SelectedChoiceId,
    bool? StudentChoice,
    string? AnswerText);
