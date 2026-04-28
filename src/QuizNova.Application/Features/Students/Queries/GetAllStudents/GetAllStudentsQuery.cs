using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed record GetAllStudentsQuery(
    string? SearchTerm = null,
    int? EnrolledCoursesCount = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<StudentDto>>>;
