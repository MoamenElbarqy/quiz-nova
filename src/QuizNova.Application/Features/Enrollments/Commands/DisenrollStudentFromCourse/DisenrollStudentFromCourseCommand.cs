using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Commands.DisenrollStudentFromCourse;

public sealed record DisenrollStudentFromCourseCommand(Guid EnrollmentId, Guid StudentId)
    : IRequest<Result<Deleted>>;
