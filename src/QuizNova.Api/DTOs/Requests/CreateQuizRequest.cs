using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateQuizRequest(
    string Title,
    Guid CourseId,
    Guid InstructorId,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    IReadOnlyCollection<CreateQuizQuestionRequest> Questions);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreateMcqRequest), "mcq")]
[JsonDerivedType(typeof(CreateTfRequest), "tf")]
public abstract record CreateQuizQuestionRequest(
    string QuestionText,
    int Marks);

public sealed record CreateMcqRequest(
    string QuestionText,
    int Marks,
    int NumberOfChoices,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceRequest> Choices)
    : CreateQuizQuestionRequest(QuestionText, Marks);

public sealed record CreateTfRequest(
    string QuestionText,
    int Marks,
    bool CorrectChoice)
    : CreateQuizQuestionRequest(QuestionText, Marks);

public sealed record CreateChoiceRequest(
    Guid Id,
    string Text,
    int DisplayOrder);
