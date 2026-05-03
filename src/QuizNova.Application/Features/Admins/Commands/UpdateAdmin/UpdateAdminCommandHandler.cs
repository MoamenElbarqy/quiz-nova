using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Admins.Commands.UpdateAdmin;

public sealed class UpdateAdminCommandHandler(
    IAppDbContext dbContext,
    ILogger<UpdateAdminCommandHandler> logger)
    : IRequestHandler<UpdateAdminCommand, Result<AdminDto>>
{
    public async Task<Result<AdminDto>> Handle(UpdateAdminCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating admin with ID: {AdminId}", request.Id);

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (admin is null)
        {
            logger.LogWarning("Admin update failed: Admin with ID {AdminId} not found", request.Id);
            return ApplicationErrors.AdminNotFound(request.Id);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            return personalInformationResult.TopError;
        }

        admin.Update(personalInformationResult.Value);

        admin.AddDomainEvent(new AdminUpdatedEvent(admin.Id));
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully updated admin {AdminId}", request.Id);

        return admin.ToAdminDto();
    }
}
