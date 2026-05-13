using MediatR;

using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;

public sealed record UpdateCourseInstructorCommand(Guid CourseId, Guid? InstructorId)
    : IRequest<Result<CourseDto>>;
