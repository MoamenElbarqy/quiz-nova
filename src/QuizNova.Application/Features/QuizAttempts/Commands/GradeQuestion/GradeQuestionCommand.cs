using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestion;

public sealed record GradeQuestionCommand(
    Guid AnswerId,
    int Score,
    string? Feedback = null)
    : IRequest<Result<Updated>>;
