using MediatR;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;

public sealed record GetQuizAttemptByIdQuery(Guid QuizAttemptId) : IRequest<Result<QuizAttemptDto>>;
