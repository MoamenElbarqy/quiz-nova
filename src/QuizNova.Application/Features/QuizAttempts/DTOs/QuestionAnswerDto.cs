using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.QuizAttempts.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "answerType")]
[JsonDerivedType(typeof(AutoGradedAnswerDto), "auto")]
[JsonDerivedType(typeof(ManuallyGradedAnswerDto), "manual")]
public abstract record QuestionAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect);

public abstract record AutoGradedAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect) : QuestionAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, IsCorrect);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "answerType")]
[JsonDerivedType(typeof(McqAnswerDto), "mcq")]
[JsonDerivedType(typeof(TfAnswerDto), "tf")]
public sealed record McqAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect,
    Guid SelectedChoiceId) : AutoGradedAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, IsCorrect);

public sealed record TfAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect,
    bool StudentChoice) : AutoGradedAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, IsCorrect);

public sealed record ManuallyGradedAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    bool IsCorrect,
    int? Score) : QuestionAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, IsCorrect);
