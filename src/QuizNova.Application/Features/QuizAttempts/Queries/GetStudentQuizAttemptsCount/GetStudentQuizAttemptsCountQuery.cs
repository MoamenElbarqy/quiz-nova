using MediatR;

using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

public sealed record GetStudentQuizAttemptsCountQuery(Guid StudentId)
    : IRequest<Result<QuizAttemptsCountDto>>;

