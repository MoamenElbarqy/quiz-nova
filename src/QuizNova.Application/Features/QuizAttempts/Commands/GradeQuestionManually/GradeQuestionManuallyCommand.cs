using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestionManually;

public sealed record GradeQuestionManuallyCommand(
    Guid AnswerId,
    int Score)
    : IRequest<Result<Updated>>;
