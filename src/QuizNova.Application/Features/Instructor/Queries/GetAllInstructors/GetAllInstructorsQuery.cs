using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

public sealed record GetAllInstructorsQuery(
    string? SearchTerm = null,
    int? CoursesCount = null,
    int? QuizzesCount = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<InstructorDto>>>;
