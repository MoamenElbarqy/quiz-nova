using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.DeleteCourseById;

public sealed record DeleteCourseByIdCommand(Guid CourseId) : IRequest<Result<Deleted>>;
