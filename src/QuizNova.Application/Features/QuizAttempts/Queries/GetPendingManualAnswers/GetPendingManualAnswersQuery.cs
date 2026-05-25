using MediatR;

using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public sealed record GetPendingManualAnswersQuery
    : IRequest<Result<IReadOnlyList<PendingManualAnswersDto>>>;
