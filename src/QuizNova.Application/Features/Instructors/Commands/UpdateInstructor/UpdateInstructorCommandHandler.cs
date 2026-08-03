using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;

public sealed class UpdateInstructorCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<UpdateInstructorCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(UpdateInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor with ID: {InstructorId}", request.Id);

        var instructor = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Instructor)
            .FirstOrDefaultAsync(ct) as Instructor;

        if (instructor is null)
        {
            logger.LogWarning("Instructor update failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        if (await mongoContext.Users.CountDocumentsAsync(
                u => u.Id != request.Id && u.PersonalInformation.Email == request.PersonalInformation.Email,
                cancellationToken: ct) > 0)
        {
            logger.LogWarning("Instructor update failed: Email {Email} already exists for another user", request.PersonalInformation.Email);
            return ApplicationErrors.UserEmailAlreadyExists(request.PersonalInformation.Email);
        }

        if (await mongoContext.Users.CountDocumentsAsync(
                u => u.Id != request.Id && u.PersonalInformation.PhoneNumber == request.PersonalInformation.PhoneNumber,
                cancellationToken: ct) > 0)
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

        await mongoContext.Users.ReplaceOneAsync(u => u.Id == instructor.Id, instructor, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Instructors], ct);

        var coursesCount = (int)await mongoContext.Courses.CountDocumentsAsync(c => c.InstructorId == request.Id, cancellationToken: ct);
        var quizzesCount = (int)await mongoContext.Quizzes.CountDocumentsAsync(q => q.InstructorId == request.Id, cancellationToken: ct);

        logger.LogInformation("Successfully updated instructor {InstructorId}", request.Id);

        return instructor.ToInstructorDto(coursesCount, quizzesCount);
    }
}
