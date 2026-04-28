using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructor.Commands.CreateInstructor;

public sealed class CreateInstructorCommandHandler(
    IAppDbContext dbContext,
    ILogger<CreateInstructorCommandHandler> logger)
    : IRequestHandler<CreateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(CreateInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating instructor with email: {Email}", request.Email);

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

        if (await dbContext.Users.AnyAsync(user => user.Id == request.Id, ct))
        {
            logger.LogWarning("Instructor creation failed: User ID {UserId} already exists", request.Id);
            return ApplicationErrors.UserIdAlreadyExists(request.Id);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.Email == request.Email, ct))
        {
            logger.LogWarning("Instructor creation failed: Email {Email} already exists", request.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.PhoneNumber == request.PhoneNumber, ct))
        {
            logger.LogWarning("Instructor creation failed: Phone number {PhoneNumber} already exists", request.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning("Instructor creation failed: Error creating personal information. Error: {ErrorDescription}", personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        var createInstructorResult = QuizNova.Domain.Entities.Users.Instructor.Create(
            request.Id,
            personalInformationResult.Value,
            new List<RefreshToken>(),
            new List<Domain.Entities.Courses.Course>(),
            new List<Domain.Entities.Quizzes.Quiz>());

        if (createInstructorResult.IsError)
        {
            logger.LogWarning("Instructor creation failed: Error creating instructor entity. Error: {ErrorDescription}", createInstructorResult.TopError.Description);
            return createInstructorResult.TopError;
        }

        await dbContext.Instructors.AddAsync(createInstructorResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created instructor {InstructorId} with email {Email}", request.Id, request.Email);

        return new InstructorDto(
            createInstructorResult.Value.Id,
            createInstructorResult.Value.PersonalInformation.Name,
            createInstructorResult.Value.PersonalInformation.Email,
            createInstructorResult.Value.PersonalInformation.Password,
            createInstructorResult.Value.PersonalInformation.PhoneNumber,
            0,
            0);
    }
}
