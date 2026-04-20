using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Commands.DeleteStudent;

public sealed record DeleteStudentCommand(Guid Id)
    : IRequest<Result<Deleted>>;
