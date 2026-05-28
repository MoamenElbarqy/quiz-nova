using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(UpdateMcqRequest), "mcq")]
[JsonDerivedType(typeof(UpdateTfRequest), "tf")]
[JsonDerivedType(typeof(UpdateEssayRequest), "essay")]
public abstract record UpdateQuestionRequest(
    string QuestionText,
    int DisplayOrder,
    int Marks);

public sealed record UpdateMcqRequest(
    string QuestionText,
    int DisplayOrder,
    int Marks,
    Guid CorrectChoiceId,
    IReadOnlyCollection<CreateChoiceRequest> Choices)
    : UpdateQuestionRequest(QuestionText, DisplayOrder, Marks);

public sealed record UpdateTfRequest(
    string QuestionText,
    int DisplayOrder,
    int Marks,
    bool CorrectChoice)
    : UpdateQuestionRequest(QuestionText, DisplayOrder, Marks);

public sealed record UpdateEssayRequest(
    string QuestionText,
    int DisplayOrder,
    int Marks,
    string? AnswerReference)
    : UpdateQuestionRequest(QuestionText, DisplayOrder, Marks);
