using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;

public sealed record DeleteInstructorCommand(Guid Id)
    : IRequest<Result<Deleted>>;
