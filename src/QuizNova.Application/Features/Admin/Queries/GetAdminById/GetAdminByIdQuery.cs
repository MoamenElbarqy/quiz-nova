using MediatR;

using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Queries.GetAdminById;

public sealed record GetAdminByIdQuery(Guid Id)
    : IRequest<Result<AdminDto>>;
