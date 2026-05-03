using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors.Events;
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
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (instructor is null)
        {
            logger.LogWarning("Instructor update failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            return personalInformationResult.TopError;
        }

        instructor.Update(personalInformationResult.Value);

        instructor.AddDomainEvent(new InstructorUpdatedEvent(instructor.Id));
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully updated instructor {InstructorId}", request.Id);

        return new InstructorDto(
            instructor.Id,
            instructor.PersonalInformation.Name,
            instructor.PersonalInformation.Email,
            instructor.PersonalInformation.Password,
            instructor.PersonalInformation.PhoneNumber,
            0,
            0);
    }
}
