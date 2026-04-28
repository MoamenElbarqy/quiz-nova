using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Application.Features.Admin.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Queries.GetAdminById;

public sealed class GetAdminByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetAdminByIdQueryHandler> logger)
    : IRequestHandler<GetAdminByIdQuery, Result<AdminDto>>
{
    public async Task<Result<AdminDto>> Handle(GetAdminByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving admin with ID: {AdminId}", request.Id);

        var admin = await dbContext.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (admin is null)
        {
            logger.LogWarning("Retrieval failed: Admin with ID {AdminId} not found", request.Id);
            return ApplicationErrors.AdminNotFound(request.Id);
        }

        logger.LogInformation("Successfully retrieved admin {AdminId}", request.Id);

        return admin.ToAdminDto();
    }
}
