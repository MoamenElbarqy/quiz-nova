namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed class ChoiceDto
{
    public Guid Id { get; init; }

    public Guid QuestionId { get; init; }

    public required string Text { get; init; }

    public int DisplayOrder { get; init; }
}
