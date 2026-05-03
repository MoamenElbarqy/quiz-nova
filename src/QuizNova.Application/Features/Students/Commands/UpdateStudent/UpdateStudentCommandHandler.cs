using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandHandler(
    IAppDbContext dbContext,
    ILogger<UpdateStudentCommandHandler> logger)
    : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating student with ID: {StudentId}", request.Id);

        var student = await dbContext.Students
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (student is null)
        {
            logger.LogWarning("Student update failed: Student with ID {StudentId} not found", request.Id);
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.Id != request.Id && user.PersonalInformation.Email == request.Email, ct))
        {
            logger.LogWarning("Student update failed: Email {Email} already exists for another user", request.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Users
                .AnyAsync(
                    user => user.Id != request.Id && user.PersonalInformation.PhoneNumber == request.PhoneNumber,
                    ct))
        {
            logger.LogWarning("Student update failed: Phone number {PhoneNumber} already exists for another user", request.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning("Student update failed: Error creating personal information. Error: {ErrorDescription}", personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        var updateStudentResult = student.Update(personalInformationResult.Value);

        if (updateStudentResult.IsError)
        {
            logger.LogWarning("Student update failed: Error updating student entity. Error: {ErrorDescription}", updateStudentResult.TopError.Description);
            return updateStudentResult.TopError;
        }

        dbContext.Students.Update(student);
        student.AddDomainEvent(new StudentUpdatedEvent(student.Id));
        await dbContext.SaveChangesAsync(ct);

        var enrolledCoursesCount = await dbContext.StudentCourses
            .CountAsync(studentCourse => studentCourse.StudentId == student.Id, ct);

        logger.LogInformation("Successfully updated student {StudentId}", request.Id);

        return new StudentDto(
            student.Id,
            student.PersonalInformation.Name,
            student.PersonalInformation.Email,
            student.PersonalInformation.Password,
            student.PersonalInformation.PhoneNumber,
            enrolledCoursesCount);
    }
}
