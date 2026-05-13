using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.RemoveStudentFromCourse;

public sealed record RemoveStudentFromCourseCommand(Guid CourseId, Guid StudentId)
    : IRequest<Result<Deleted>>;
