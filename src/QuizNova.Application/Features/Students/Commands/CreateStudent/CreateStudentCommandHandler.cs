using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.Student.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandHandler(
    IAppDbContext dbContext,
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

        if (await dbContext.Users.AnyAsync(user => user.Id == request.Id, ct))
        {
            logger.LogWarning("Student creation failed: User ID {UserId} already exists", request.Id);
            return ApplicationErrors.UserIdAlreadyExists(request.Id);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.Email == request.Email, ct))
        {
            logger.LogWarning("Student creation failed: Email {Email} already exists", request.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.PersonalInformation.PhoneNumber == request.PhoneNumber, ct))
        {
            logger.LogWarning("Student creation failed: Phone number {PhoneNumber} already exists", request.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning("Student creation failed: Error creating personal information. Error: {ErrorDescription}", personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        var createStudentResult = Student.Create(
            request.Id,
            personalInformationResult.Value,
            new List<RefreshToken>(),
            new List<Domain.Entities.StudentCourses.StudentCourse>(),
            new List<Domain.Entities.QuizAttempts.QuizAttempt>());

        if (createStudentResult.IsError)
        {
            logger.LogWarning("Student creation failed: Error creating student entity. Error: {ErrorDescription}", createStudentResult.TopError.Description);
            return createStudentResult.TopError;
        }

        await dbContext.Students.AddAsync(createStudentResult.Value, ct);
        createStudentResult.Value.AddDomainEvent(new StudentCreatedEvent(createStudentResult.Value.Id));
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created student {StudentId} with email {Email}", request.Id, request.Email);

        return new StudentDto(
            createStudentResult.Value.Id,
            createStudentResult.Value.PersonalInformation.Name,
            createStudentResult.Value.PersonalInformation.Email,
            createStudentResult.Value.PersonalInformation.Password,
            createStudentResult.Value.PersonalInformation.PhoneNumber,
            0);
    }
}
