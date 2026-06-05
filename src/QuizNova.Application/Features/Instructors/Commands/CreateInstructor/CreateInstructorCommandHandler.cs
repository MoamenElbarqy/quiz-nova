using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructors.Commands.CreateInstructor;

public sealed class CreateInstructorCommandHandler(
    IAppDbContext dbContext,
    IIdentityService identityService,
    ILogger<CreateInstructorCommandHandler> logger)
    : IRequestHandler<CreateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(CreateInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating instructor with email: {Email}", request.PersonalInformation.Email);

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            logger.LogWarning("Instructor creation failed: Invalid role {Role}", request.Role);
            return ApplicationErrors.UserRoleInvalid(request.Role);
        }

        if (role != UserRole.Instructor)
        {
            logger.LogWarning("Instructor creation failed: Role {Role} is not Instructor", request.Role);
            return ApplicationErrors.CreateInstructorRoleInvalid(request.Role);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.Email == request.PersonalInformation.Email, ct))
        {
            logger.LogWarning("Instructor creation failed: Email {Email} already exists", request.PersonalInformation.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.PersonalInformation.Email);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.PhoneNumber == request.PersonalInformation.PhoneNumber, ct))
        {
            logger.LogWarning("Instructor creation failed: Phone number {PhoneNumber} already exists",
                request.PersonalInformation.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PersonalInformation.PhoneNumber);
        }

        // 1. Register User in Identity Database
        var identityResult = await identityService.RegisterUserAsync(
            request.PersonalInformation.Email,
            request.Password,
            request.PersonalInformation.Name,
            nameof(UserRole.Instructor),
            ct);

        if (identityResult.IsError)
        {
            logger.LogWarning("Instructor creation failed: Error registering identity user. Error: {ErrorDescription}",
                identityResult.TopError.Description);
            return identityResult.Errors;
        }

        var userId = Guid.Parse(identityResult.Value);

        // 2. Create PersonalInformation Domain Value Object
        var personalInformationResult = PersonalInformation.Create(
            request.PersonalInformation.Name,
            request.PersonalInformation.Email,
            request.PersonalInformation.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning(
                "Instructor creation failed: Error creating personal information. Error: {ErrorDescription}",
                personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        // 3. Create Instructor Domain Aggregate
        var createInstructorResult = Instructor.Create(
            userId,
            personalInformationResult.Value,
            [],
            []);

        if (createInstructorResult.IsError)
        {
            logger.LogWarning("Instructor creation failed: Error creating instructor entity. Error: {ErrorDescription}",
                createInstructorResult.TopError.Description);
            return createInstructorResult.TopError;
        }

        await dbContext.Instructors.AddAsync(createInstructorResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created instructor {InstructorId} with email {Email}",
            createInstructorResult.Value.Id,
            request.PersonalInformation.Email);

        return createInstructorResult.Value.ToInstructorDto(0, 0);
    }
}
