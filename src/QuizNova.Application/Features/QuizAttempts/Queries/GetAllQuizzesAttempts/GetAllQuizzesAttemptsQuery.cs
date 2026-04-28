using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public sealed record GetAllQuizzesAttemptsQuery(
    string? SearchTerm = null,
    int? CorrectAnswers = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<QuizAttemptDto>>>;
