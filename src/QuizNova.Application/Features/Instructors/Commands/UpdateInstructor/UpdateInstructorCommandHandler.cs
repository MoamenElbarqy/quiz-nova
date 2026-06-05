using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;

public sealed class UpdateInstructorCommandHandler(
    IAppDbContext dbContext,
    ILogger<UpdateInstructorCommandHandler> logger)
    : IRequestHandler<UpdateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(UpdateInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor with ID: {InstructorId}", request.Id);

        var instructor = await dbContext.Instructors
            .Include(i => i.Courses)
            .Include(i => i.Quizzes)
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (instructor is null)
        {
            logger.LogWarning("Instructor update failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        if (await dbContext.Users.AnyAsync(
                user => user.Id != request.Id && user.PersonalInformation.Email == request.PersonalInformation.Email, ct))
        {
            logger.LogWarning("Instructor update failed: Email {Email} already exists for another user", request.PersonalInformation.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.PersonalInformation.Email);
        }

        if (await dbContext.Users.AnyAsync(
                user => user.Id != request.Id && user.PersonalInformation.PhoneNumber == request.PersonalInformation.PhoneNumber, ct))
        {
            logger.LogWarning(
                "Instructor update failed: Phone number {PhoneNumber} already exists for another user",
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

        var updateInstructorResult = instructor.Update(personalInformationResult.Value);

        if (updateInstructorResult.IsError)
        {
            logger.LogWarning(
                "Instructor update failed: Error updating instructor entity. Error: {ErrorDescription}",
                updateInstructorResult.TopError.Description);
            return updateInstructorResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully updated instructor {InstructorId}", request.Id);

        return instructor.ToInstructorDto(instructor.Courses.Count(), instructor.Quizzes.Count());
    }
}
