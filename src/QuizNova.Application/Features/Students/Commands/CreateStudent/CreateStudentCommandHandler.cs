using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandHandler(
    IAppDbContext dbContext,
    IIdentityService identityService,
    ILogger<CreateStudentCommandHandler> logger)
    : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating student with email: {Email}", request.Email);

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            logger.LogWarning("Student creation failed: Invalid role {Role}", request.Role);
            return ApplicationErrors.UserRoleInvalid(request.Role);
        }

        if (role != UserRole.Student)
        {
            logger.LogWarning("Student creation failed: Role {Role} is not Student", request.Role);
            return ApplicationErrors.CreateStudentRoleInvalid(request.Role);
        }

        if (await dbContext.Users.AnyAsync(u => u.PersonalInformation.PhoneNumber == request.PhoneNumber, ct))
        {
            logger.LogWarning(
                "Student creation failed: Phone number {PhoneNumber} already exists",
                request.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        // 1. Register User in Identity Database
        var identityResult = await identityService.RegisterUserAsync(
            request.Email,
            request.Password,
            request.Name,
            nameof(UserRole.Student),
            ct);

        if (identityResult.IsError)
        {
            logger.LogWarning("Student creation failed: Error registering identity user. Error: {ErrorDescription}",
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
            logger.LogWarning(
                "Student creation failed: Error creating personal information. Error: {ErrorDescription}",
                personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        // 3. Create Student Domain Aggregate
        var createStudentResult = Student.Create(
            userId,
            personalInformationResult.Value,
            [],
            []);

        if (createStudentResult.IsError)
        {
            logger.LogWarning(
                "Student creation failed: Error creating student entity. Error: {ErrorDescription}",
                createStudentResult.TopError.Description);
            return createStudentResult.TopError;
        }

        await dbContext.Students.AddAsync(createStudentResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created student {StudentId} with email {Email}",
            createStudentResult.Value.Id, request.Email);

        return createStudentResult.Value.ToStudentDto(0);
    }
}
