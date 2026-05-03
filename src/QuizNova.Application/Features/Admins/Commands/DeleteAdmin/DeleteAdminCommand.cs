using MediatR;

using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admins.Commands.DeleteAdmin;

public sealed record DeleteAdminCommand(Guid Id)
    : IRequest<Result<Deleted>>;
