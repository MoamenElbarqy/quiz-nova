using MediatR;

using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.CompleteQuizAttempt;

public sealed record CompleteQuizAttemptCommand(
    Guid AttemptId,
    DateTimeOffset SubmittedAt) : IRequest<Result<QuizAttemptDto>>;
