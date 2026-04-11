namespace QuizNova.Application.Features.Quizzes.DTOs;

public class QuestionDto
{
    public Guid Id { get; init; }

    public Guid QuizId { get; init; }

    public string QuestionText { get; init; } = string.Empty;

    public int Marks { get; init; }
}
