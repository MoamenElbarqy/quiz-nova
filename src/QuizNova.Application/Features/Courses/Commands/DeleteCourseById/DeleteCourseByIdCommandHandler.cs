using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;

namespace QuizNova.Application.Features.Courses.Commands.DeleteCourseById;

public sealed class DeleteCourseByIdCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<DeleteCourseByIdCommandHandler> logger,
    ICacheInvalidator cacheInvalidator,
    IUser currentUser)
    : IRequestHandler<DeleteCourseByIdCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteCourseByIdCommand request, CancellationToken ct)
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

        logger.LogInformation("Deleting course with ID: {CourseId}", request.CourseId);

        var course = await mongoContext.Courses
            .Find(c => c.Id == request.CourseId)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Course deletion failed: Course with ID {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        var deleteResult = course.Delete();
        if (deleteResult.IsError)
        {
            logger.LogWarning("Course deletion failed: {ErrorDescription}", deleteResult.TopError.Description);
            return deleteResult.TopError;
        }

        await mongoContext.Courses.DeleteOneAsync(c => c.Id == request.CourseId, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Courses], ct);

        logger.LogInformation("Successfully deleted course {CourseId}", request.CourseId);

        return Result.Deleted;
    }
}
