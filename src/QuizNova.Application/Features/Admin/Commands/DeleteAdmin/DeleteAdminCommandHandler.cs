using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Commands.DeleteAdmin;

public sealed class DeleteAdminCommandHandler(
    IAppDbContext dbContext,
    ILogger<DeleteAdminCommandHandler> logger)
    : IRequestHandler<DeleteAdminCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteAdminCommand request, CancellationToken ct)
    {
        logger.LogInformation("Deleting admin with ID: {AdminId}", request.Id);

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (admin is null)
        {
            logger.LogWarning("Admin deletion failed: Admin with ID {AdminId} not found", request.Id);
            return ApplicationErrors.AdminNotFound(request.Id);
        }

        dbContext.Admins.Remove(admin);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully deleted admin {AdminId}", request.Id);

        return Result.Deleted;
    }
}
