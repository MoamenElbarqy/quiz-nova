using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesById;

public sealed record GetInstructorCoursesByIdQuery(Guid InstructorId)
    : IRequest<Result<List<CourseDto>>>;


