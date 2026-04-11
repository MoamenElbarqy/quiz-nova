using MediatR;

using QuizNova.Application.Features.Departments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Departments.Queries.GetCollegeDepartments;

public sealed record GetAllDepartmentsQuery()
    : IRequest<Result<List<DepartmentDto>>>;
