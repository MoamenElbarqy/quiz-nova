using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructor.Commands.UpdateInstructor;

public sealed class UpdateInstructorCommandHandler(
    IAppDbContext dbContext,
    ILogger<UpdateInstructorCommandHandler> logger)
    : IRequestHandler<UpdateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(UpdateInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor with ID: {InstructorId}", request.Id);

        var instructor = await dbContext.Instructors
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (instructor is null)
        {
            logger.LogWarning("Instructor update failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        if (await dbContext.Users
                .AnyAsync(user => user.Id != request.Id && user.PersonalInformation.Email == request.Email, ct))
        {
            logger.LogWarning("Instructor update failed: Email {Email} already exists for another user", request.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Users.AnyAsync(
                    user => user.Id != request.Id && user.PersonalInformation.PhoneNumber == request.PhoneNumber,
                    ct))
        {
            logger.LogWarning(
                "Instructor update failed: Phone number {PhoneNumber} already exists for another user",
                request.PhoneNumber);
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            logger.LogWarning(
                "Instructor update failed: Error creating personal information. Error: {ErrorDescription}",
                personalInformationResult.TopError.Description);
            return personalInformationResult.TopError;
        }

        var updateInstructorResult = instructor.Update(personalInformationResult.Value);

        if (updateInstructorResult.IsError)
        {
            logger.LogWarning(
                "Instructor update failed: Error updating instructor entity. Error: {ErrorDescription}",
                updateInstructorResult.TopError.Description);
            return updateInstructorResult.TopError;
        }

        dbContext.Instructors.Update(instructor);
        await dbContext.SaveChangesAsync(ct);

        var coursesCount = await dbContext.Courses
            .CountAsync(course => course.InstructorId == instructor.Id, ct);

        var quizzesCount = await dbContext.Quizzes
            .CountAsync(quiz => quiz.InstructorId == instructor.Id, ct);

        logger.LogInformation("Successfully updated instructor {InstructorId}", request.Id);

        return new InstructorDto(
            instructor.Id,
            instructor.PersonalInformation.Name,
            instructor.PersonalInformation.Email,
            instructor.PersonalInformation.Password,
            instructor.PersonalInformation.PhoneNumber,
            coursesCount,
            quizzesCount);
    }
}
