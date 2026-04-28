using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.Quizzes.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(McqDto), "mcq")]
[JsonDerivedType(typeof(TfDto), "tf")]
public abstract class QuestionDto
{
    public Guid Id { get; init; }

    public Guid QuizId { get; init; }

    public string QuestionText { get; init; } = string.Empty;

    public int Marks { get; init; }

    public string Type { get; init; } = string.Empty;
}

public sealed class McqDto : QuestionDto
{
    public int NumberOfChoices { get; init; }

    public Guid CorrectChoiceId { get; init; }

    public IReadOnlyCollection<ChoiceDto> Choices { get; init; } = Array.Empty<ChoiceDto>();
}

public sealed class TfDto : QuestionDto
{
    public bool CorrectChoice { get; init; }
}
