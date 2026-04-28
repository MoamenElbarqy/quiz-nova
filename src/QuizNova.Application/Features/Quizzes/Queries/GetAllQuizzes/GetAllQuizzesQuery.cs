using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed record GetAllQuizzesQuery(
    string? SearchTerm = null,
    int? Marks = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<QuizDto>>>;
