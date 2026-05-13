using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.CreateCourse;

public sealed record CreateCourseCommand(
    Guid Id,
    string Name,
    Guid? InstructorId,
    int MinimumPassingMarks,
    int MaximumMarks)
    : IRequest<Result<CourseDto>>;
