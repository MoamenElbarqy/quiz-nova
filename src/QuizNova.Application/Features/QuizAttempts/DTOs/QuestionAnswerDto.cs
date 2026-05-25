using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.QuizAttempts.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "answerType")]
[JsonDerivedType(typeof(AutoGradedAnswerDto), "auto")]
[JsonDerivedType(typeof(ManuallyGradedAnswerDto), "manual")]
public abstract record QuestionAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "autoAnswerType")]
[JsonDerivedType(typeof(McqAnswerDto), "mcq")]
[JsonDerivedType(typeof(TfAnswerDto), "tf")]
public abstract record AutoGradedAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    string AutoAnswerType,
    bool IsCorrect) : QuestionAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType);

public sealed record McqAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    string AutoAnswerType,
    bool IsCorrect,
    Guid SelectedChoiceId) : AutoGradedAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, AutoAnswerType, IsCorrect);

public sealed record TfAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    string AutoAnswerType,
    bool IsCorrect,
    bool StudentChoice) : AutoGradedAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType, AutoAnswerType, IsCorrect);

public sealed record ManuallyGradedAnswerDto(
    Guid AnswerId,
    Guid QuestionId,
    string QuestionText,
    string AnswerType,
    int? Score,
    string StudentResponse,
    string? Feedback) : QuestionAnswerDto(AnswerId, QuestionId, QuestionText, AnswerType);
