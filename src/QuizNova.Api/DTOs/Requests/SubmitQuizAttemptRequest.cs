using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

public sealed record SubmitQuizAttemptRequest(
    Guid QuizId,
    DateTimeOffset StartedAt,
    DateTimeOffset SubmittedAt,
    IReadOnlyCollection<SubmitQuestionAnswerRequest> QuestionAnswers);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SubmitMcqAnswerRequest), "mcq")]
[JsonDerivedType(typeof(SubmitTfAnswerRequest), "tf")]
[JsonDerivedType(typeof(SubmitEssayAnswerRequest), "essay")]
public abstract record SubmitQuestionAnswerRequest(
    Guid QuestionId);

public sealed record SubmitMcqAnswerRequest(
    Guid QuestionId,
    Guid SelectedChoiceId)
    : SubmitQuestionAnswerRequest(QuestionId);

public sealed record SubmitTfAnswerRequest(
    Guid QuestionId,
    bool StudentChoice)
    : SubmitQuestionAnswerRequest(QuestionId);

public sealed record SubmitEssayAnswerRequest(
    Guid QuestionId,
    string StudentResponse)
    : SubmitQuestionAnswerRequest(QuestionId);
