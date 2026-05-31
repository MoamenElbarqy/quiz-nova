using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Commands.RemoveStudentFromCourse;

public sealed record RemoveStudentFromCourseCommand(Guid EnrollmentId, Guid StudentId)
    : IRequest<Result<Deleted>>;
