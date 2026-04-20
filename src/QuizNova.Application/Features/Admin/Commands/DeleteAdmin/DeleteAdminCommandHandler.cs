using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Commands.DeleteAdmin;

public sealed class DeleteAdminCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<DeleteAdminCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteAdminCommand request, CancellationToken ct)
    {
        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (admin is null)
        {
            return ApplicationErrors.AdminNotFound(request.Id);
        }

        dbContext.Admins.Remove(admin);
        await dbContext.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}
