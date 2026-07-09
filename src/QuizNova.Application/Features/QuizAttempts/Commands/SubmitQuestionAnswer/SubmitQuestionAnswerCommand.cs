using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuestionAnswer;

public sealed record SubmitQuestionAnswerCommand(
    Guid AttemptId,
    SubmitAnswerCommand Answer) : IRequest<Result<Submitted>>;

public abstract record SubmitAnswerCommand(Guid QuestionId);

public sealed record SubmitMcqAnswerCommand(
    Guid QuestionId,
    Guid SelectedChoiceId)
    : SubmitAnswerCommand(QuestionId);

public sealed record SubmitTfAnswerCommand(
    Guid QuestionId,
    bool StudentChoice)
    : SubmitAnswerCommand(QuestionId);

public sealed record SubmitEssayAnswerCommand(
    Guid QuestionId,
    string StudentResponse)
    : SubmitAnswerCommand(QuestionId);
