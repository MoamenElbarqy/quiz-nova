using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.EnrollStudentInCourse;

public sealed record EnrollStudentInCourseCommand(Guid CourseId, Guid StudentId)
    : IRequest<Result<Created>>;
