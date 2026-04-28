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
[JsonDerivedType(typeof(SubmitTfAnswerRequest), "tf")]
public abstract record SubmitQuestionAnswerRequest(
    Guid Id,
    Guid QuestionId);

public sealed record SubmitMcqAnswerRequest(
    Guid Id,
    Guid QuestionId,
    Guid SelectedChoiceId)
    : SubmitQuestionAnswerRequest(Id, QuestionId);

public sealed record SubmitTfAnswerRequest(
    Guid Id,
    Guid QuestionId,
    bool StudentChoice)
    : SubmitQuestionAnswerRequest(Id, QuestionId);
