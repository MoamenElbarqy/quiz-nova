using MediatR;

using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.StartQuizAttempt;

public sealed record StartQuizAttemptCommand(Guid QuizId) : IRequest<Result<QuizAttemptDto>>;
