using MediatR;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery(
    string? SearchTerm = null,
    int? EnrolledStudentsCount = null,
    int? QuizzesCount = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<CourseDto>>>;
