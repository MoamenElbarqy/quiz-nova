using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public sealed record GetPendingManualAnswersQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<PendingManualAnswersDto>>>;
