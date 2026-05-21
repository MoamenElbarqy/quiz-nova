using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Admins.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(
    IAppDbContext dbContext,
    IIdentityService identityService,
    ILogger<CreateAdminCommandHandler> logger)
    : IRequestHandler<CreateAdminCommand, Result<AdminDto>>
{
    public async Task<Result<AdminDto>> Handle(CreateAdminCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating admin with email: {Email}", request.Email);

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            logger.LogWarning("Admin creation failed: Invalid role {Role}", request.Role);
            return ApplicationErrors.UserRoleInvalid(request.Role);
        }

        if (role != UserRole.Admin)
        {
            logger.LogWarning("Admin creation failed: Role {Role} is not Admin", request.Role);
            return ApplicationErrors.CreateAdminRoleInvalid(request.Role);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.Email == request.Email, ct))
        {
            logger.LogWarning("Admin creation failed: Email {Email} already exists", request.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.PhoneNumber == request.PhoneNumber, ct))
        {
            logger.LogWarning("Admin creation failed: Phone number {PhoneNumber} already exists", request.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        // 1. Register User in Identity Database
        var identityResult = await identityService.RegisterUserAsync(
            request.Email,
            request.Password,
            request.Name,
            nameof(UserRole.Admin),
            ct);

        if (identityResult.IsError)
        {
            logger.LogWarning("Admin creation failed: Error registering identity user. Error: {ErrorDescription}",
                identityResult.TopError.Description);
            return identityResult.Errors;
        }

        var userId = Guid.Parse(identityResult.Value);

        // 2. Create PersonalInformation Domain Value Object
        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning("Admin creation failed: Error creating personal information. Error: {ErrorDescription}",
                personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        // 3. Create Admin Domain Aggregate
        var createAdminResult = Admin.Create(
            userId,
            personalInformationResult.Value);

        if (createAdminResult.IsError)
        {
            logger.LogWarning("Admin creation failed: Error creating admin entity. Error: {ErrorDescription}",
                createAdminResult.TopError.Description);
            return createAdminResult.TopError;
        }

        await dbContext.Admins.AddAsync(createAdminResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created admin {AdminId} with email {Email}", createAdminResult.Value.Id,
            request.Email);

        return createAdminResult.Value.ToAdminDto();
    }
}
