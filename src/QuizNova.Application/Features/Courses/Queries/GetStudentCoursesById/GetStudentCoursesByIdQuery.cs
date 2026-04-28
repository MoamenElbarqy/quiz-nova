using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesById;

public sealed record GetStudentCoursesByIdQuery(Guid StudentId)
    : IRequest<Result<List<StudentCourseDto>>>;
