using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Admins.Commands.UpdateAdmin;

public sealed class UpdateAdminCommandHandler(
    IAppDbContext dbContext,
    ILogger<UpdateAdminCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
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

        if (await dbContext.Users.AnyAsync(
                user => user.Id != request.Id && user.PersonalInformation.Email == request.PersonalInformation.Email, ct))
        {
            logger.LogWarning("Admin update failed: Email {Email} already exists for another user", request.PersonalInformation.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.PersonalInformation.Email);
        }

        if (await dbContext.Users.AnyAsync(
                user => user.Id != request.Id && user.PersonalInformation.PhoneNumber == request.PersonalInformation.PhoneNumber, ct))
        {
            logger.LogWarning(
                "Admin update failed: Phone number {PhoneNumber} already exists for another user",
                request.PersonalInformation.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PersonalInformation.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.PersonalInformation.Name,
            request.PersonalInformation.Email,
            request.PersonalInformation.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            return personalInformationResult.TopError;
        }

        var updateAdminResult = admin.Update(personalInformationResult.Value);

        if (updateAdminResult.IsError)
        {
            logger.LogWarning(
                "Admin update failed: Error updating admin entity. Error: {ErrorDescription}",
                updateAdminResult.TopError.Description);
            return updateAdminResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["admins"], ct);

        logger.LogInformation("Successfully updated admin {AdminId}", request.Id);

        return admin.ToAdminDto();
    }
}
