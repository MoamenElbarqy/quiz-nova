using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;

public sealed class DeleteInstructorCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<DeleteInstructorCommandHandler> logger,
    ICacheInvalidator cacheInvalidator,
    IUser currentUser)
    : IRequestHandler<DeleteInstructorCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteInstructorCommand request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var currentUserId))
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var isExecutingUserAdmin = await mongoContext.Users
            .Find(u => u.Id == currentUserId && u is Admin)
            .AnyAsync(ct);

        if (!isExecutingUserAdmin)
        {
            return ApplicationErrors.AdminNotFound(currentUserId);
        }

        logger.LogInformation("Deleting instructor with ID: {InstructorId}", request.Id);

        var instructor = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Instructor)
            .FirstOrDefaultAsync(ct) as Instructor;

        if (instructor is null)
        {
            logger.LogWarning("Instructor deletion failed: Instructor with ID {InstructorId} not found", request.Id);
            return ApplicationErrors.InstructorNotFound(request.Id);
        }

        var deleteResult = instructor.Delete();
        if (deleteResult.IsError)
        {
            logger.LogWarning("Instructor deletion failed: {ErrorDescription}", deleteResult.TopError.Description);
            return deleteResult.TopError;
        }

        await mongoContext.Users.DeleteOneAsync(u => u.Id == request.Id, ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Instructors], ct);

        logger.LogInformation("Successfully deleted instructor {InstructorId}", request.Id);

        return Result.Deleted;
    }
}
