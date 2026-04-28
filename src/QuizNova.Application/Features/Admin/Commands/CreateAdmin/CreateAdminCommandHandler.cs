using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Application.Features.Admin.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Admin.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(
    IAppDbContext dbContext,
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

        if (await dbContext.Users.AnyAsync(user => user.Id == request.Id, ct))
        {
            logger.LogWarning("Admin creation failed: User ID {UserId} already exists", request.Id);
            return ApplicationErrors.UserIdAlreadyExists(request.Id);
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

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning("Admin creation failed: Error creating personal information. Error: {ErrorDescription}", personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        var createAdminResult = Domain.Entities.Users.Admin.Create(
            request.Id,
            personalInformationResult.Value,
            new List<RefreshToken>());

        if (createAdminResult.IsError)
        {
            logger.LogWarning("Admin creation failed: Error creating admin entity. Error: {ErrorDescription}", createAdminResult.TopError.Description);
            return createAdminResult.TopError;
        }

        await dbContext.Admins.AddAsync(createAdminResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created admin {AdminId} with email {Email}", request.Id, request.Email);

        return createAdminResult.Value.ToAdminDto();
    }
}
