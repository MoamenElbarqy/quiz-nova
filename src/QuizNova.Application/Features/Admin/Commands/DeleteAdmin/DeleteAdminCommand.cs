using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Commands.DeleteAdmin;

public sealed record DeleteAdminCommand(Guid Id)
    : IRequest<Result<Deleted>>;
