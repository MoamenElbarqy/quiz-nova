using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.DeleteQuestion;

public sealed record DeleteQuestionCommand(
    Guid QuizId,
    Guid QuestionId)
    : IRequest<Result<Deleted>>;
