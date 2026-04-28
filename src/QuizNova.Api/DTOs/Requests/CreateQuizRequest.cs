using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateQuizRequest(
    Guid Id,
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
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks);

public sealed record CreateMcqRequest(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks,
    int NumberOfChoices,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceRequest> Choices)
    : CreateQuizQuestionRequest(Id, QuizId, QuestionText, Marks);

public sealed record CreateTfRequest(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    int Marks,
    bool CorrectChoice)
    : CreateQuizQuestionRequest(Id, QuizId, QuestionText, Marks);

public sealed record CreateChoiceRequest(
    Guid Id,
    Guid QuestionId,
    string Text,
    int DisplayOrder);
