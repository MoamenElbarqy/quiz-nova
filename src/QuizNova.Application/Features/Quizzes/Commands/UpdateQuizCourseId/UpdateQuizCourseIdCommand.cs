using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizCourseId;

public sealed record UpdateQuizCourseIdCommand(
    Guid QuizId,
    Guid NewCourseId)
    : IRequest<Result<Updated>>;
