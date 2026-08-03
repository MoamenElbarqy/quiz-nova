using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;

namespace QuizNova.Application.Features.Admins.Queries.GetAdminById;

public sealed class GetAdminByIdQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAdminByIdQueryHandler> logger)
    : IRequestHandler<GetAdminByIdQuery, Result<AdminDto>>
{
    public async Task<Result<AdminDto>> Handle(GetAdminByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving admin with ID: {AdminId}", request.Id);

        var admin = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Admin)
            .FirstOrDefaultAsync(ct) as Admin;

        if (admin is null)
        {
            logger.LogWarning("Retrieval failed: Admin with ID {AdminId} not found", request.Id);
            return ApplicationErrors.AdminNotFound(request.Id);
        }

        logger.LogInformation("Successfully retrieved admin {AdminId}", request.Id);

        return admin.ToAdminDto();
    }
}
