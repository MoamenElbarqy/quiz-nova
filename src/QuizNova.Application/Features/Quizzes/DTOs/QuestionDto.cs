using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.Quizzes.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(McqDto), "mcq")]
[JsonDerivedType(typeof(TfDto), "tf")]
public abstract class QuestionDto
{
    public Guid Id { get; init; }

    public Guid QuizId { get; init; }

    public required string QuestionText { get; init; }

    public int Marks { get; init; }
}

public sealed class McqDto : QuestionDto
{
    public int NumberOfChoices { get; init; }

    public Guid CorrectChoiceId { get; init; }

    public IReadOnlyCollection<ChoiceDto> Choices { get; init; } = [];
}

public sealed class TfDto : QuestionDto
{
    public bool CorrectChoice { get; init; }
}
