using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.QuizAttempts.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "answerType")]
[JsonDerivedType(typeof(McqAnswerDto), "mcq")]
[JsonDerivedType(typeof(TfAnswerDto), "tf")]
public abstract record QuestionAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect);

public sealed record McqAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect,
    Guid SelectedChoiceId) : QuestionAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, IsCorrect);

public sealed record TfAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect,
    bool StudentChoice) : QuestionAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, IsCorrect);
