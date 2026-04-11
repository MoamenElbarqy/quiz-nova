namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed class ChoiceDto
{
    public Guid Id { get; init; }

    public Guid QuestionId { get; init; }

    public string Text { get; init; } = string.Empty;

    public int DisplayOrder { get; init; }
}
