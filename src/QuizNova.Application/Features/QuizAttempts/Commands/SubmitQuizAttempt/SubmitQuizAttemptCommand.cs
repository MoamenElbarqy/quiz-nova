using MediatR;

using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed record SubmitQuizAttemptCommand(
    Guid QuizAttemptId,
    Guid StudentId,
    Guid QuizId,
    DateTimeOffset StartedAt,
    DateTimeOffset SubmittedAt,
    IReadOnlyCollection<SubmitQuestionAnswerCommand> QuestionAnswers)
    : IRequest<Result<QuizAttemptDto>>;

public abstract record SubmitQuestionAnswerCommand(Guid QuestionId);

public sealed record SubmitMcqAnswerCommand(
    Guid QuestionId,
    Guid SelectedChoiceId)
    : SubmitQuestionAnswerCommand(QuestionId);

public sealed record SubmitTrueFalseQuestionAnswerCommand(
    Guid QuestionId,
    bool StudentChoice)
    : SubmitQuestionAnswerCommand(QuestionId);
