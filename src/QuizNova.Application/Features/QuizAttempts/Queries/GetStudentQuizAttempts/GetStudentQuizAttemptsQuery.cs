using MediatR;

using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

public sealed record GetStudentQuizAttemptsQuery(Guid StudentId)
    : IRequest<Result<List<QuizAttemptDto>>>;
