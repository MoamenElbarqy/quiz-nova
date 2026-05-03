using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors.Events;

namespace QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;

public sealed class DeleteInstructorCommandHandler(
    IAppDbContext dbContext,
    ILogger<DeleteInstructorCommandHandler> logger)
    : IRequestHandler<DeleteInstructorCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Deleting instructor with ID: {InstructorId}", request.Id);

        var instructor = await dbContext.Instructors
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (instructor is null)
        {
            logger.LogWarning("Instructor deletion failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        dbContext.Instructors.Remove(instructor);
        instructor.AddDomainEvent(new InstructorDeletedEvent(instructor.Id));
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully deleted instructor {InstructorId}", request.Id);

        return Result.Deleted;
    }
}
