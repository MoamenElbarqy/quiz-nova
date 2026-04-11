using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery()
    : IRequest<Result<List<CourseDto>>>;
