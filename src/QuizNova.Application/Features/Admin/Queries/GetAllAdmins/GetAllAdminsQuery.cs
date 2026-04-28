using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Queries.GetAllAdmins;

public sealed record GetAllAdminsQuery(
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<AdminDto>>>;
