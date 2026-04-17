namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed record StudentQuizzesDto(
    DateTimeOffset ServerUtc,
    IReadOnlyList<StudentQuizDto> Quizzes);
