using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

public sealed record SubmitQuizAttemptRequest(
    Guid Id,
    Guid QuizId,
    DateTimeOffset StartedAt,
    DateTimeOffset SubmittedAt,
    IReadOnlyCollection<SubmitQuestionAnswerRequest> QuestionAnswers);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SubmitMcqAnswerRequest), "mcq")]
[JsonDerivedType(typeof(SubmitTrueFalseQuestionAnswerRequest), "true-false")]
public abstract record SubmitQuestionAnswerRequest(
    Guid QuestionId);

public sealed record SubmitMcqAnswerRequest(
    Guid QuestionId,
    Guid SelectedChoiceId)
    : SubmitQuestionAnswerRequest(QuestionId);

public sealed record SubmitTrueFalseQuestionAnswerRequest(
    Guid QuestionId,
    bool StudentChoice)
    : SubmitQuestionAnswerRequest(QuestionId);
