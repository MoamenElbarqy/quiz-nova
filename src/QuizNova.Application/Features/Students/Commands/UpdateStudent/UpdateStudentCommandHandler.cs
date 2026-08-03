using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<UpdateStudentCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating student with ID: {StudentId}", request.Id);

        var student = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Student)
            .FirstOrDefaultAsync(ct) as Student;

        if (student is null)
        {
            logger.LogWarning("Student update failed: Student with ID {StudentId} not found", request.Id);
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        if (await mongoContext.Users.CountDocumentsAsync(
                u => u.Id != request.Id && u.PersonalInformation.Email == request.PersonalInformation.Email,
                cancellationToken: ct) > 0)
        {
            logger.LogWarning("Student update failed: Email {Email} already exists for another user", request.PersonalInformation.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.PersonalInformation.Email);
        }

        if (await mongoContext.Users.CountDocumentsAsync(
                u => u.Id != request.Id && u.PersonalInformation.PhoneNumber == request.PersonalInformation.PhoneNumber,
                cancellationToken: ct) > 0)
        {
            logger.LogWarning(
                "Student update failed: Phone number {PhoneNumber} already exists for another user",
                request.PersonalInformation.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PersonalInformation.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.PersonalInformation.Name,
            request.PersonalInformation.Email,
            request.PersonalInformation.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning(
                "Student update failed: Error creating personal information. Error: {ErrorDescription}",
                personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        var updateStudentResult = student.Update(personalInformationResult.Value);

        if (updateStudentResult.IsError)
        {
            logger.LogWarning(
                "Student update failed: Error updating student entity. Error: {ErrorDescription}",
                updateStudentResult.TopError.Description);
            return updateStudentResult.TopError;
        }

        await mongoContext.Users.ReplaceOneAsync(u => u.Id == student.Id, student, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Students], ct);

        var enrollmentCount = (int)await mongoContext.Enrollments
            .CountDocumentsAsync(e => e.StudentId == request.Id, cancellationToken: ct);

        logger.LogInformation("Successfully updated student {StudentId}", request.Id);

        return student.ToStudentDto(enrollmentCount);
    }
}
