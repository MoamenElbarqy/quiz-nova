using MediatR;

using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Queries.GetAllAdmins;

public sealed record GetAllAdminsQuery
    : IRequest<Result<List<AdminDto>>>;
