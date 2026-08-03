using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Students.Commands.DeleteStudent;

public sealed class DeleteStudentCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<DeleteStudentCommandHandler> logger,
    ICacheInvalidator cacheInvalidator,
    IUser currentUser)
    : IRequestHandler<DeleteStudentCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteStudentCommand request, CancellationToken ct)
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

        logger.LogInformation("Deleting student with ID: {StudentId}", request.Id);

        var student = await mongoContext.Users
            .Find(u => u.Id == request.Id && u is Student)
            .FirstOrDefaultAsync(ct) as Student;

        if (student is null)
        {
            logger.LogWarning("Student deletion failed: Student with ID {StudentId} not found", request.Id);
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        var deleteResult = student.Delete();
        if (deleteResult.IsError)
        {
            logger.LogWarning("Student deletion failed: {ErrorDescription}", deleteResult.TopError.Description);
            return deleteResult.TopError;
        }

        await mongoContext.Users.DeleteOneAsync(u => u.Id == request.Id, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Students, $"students:{request.Id}"], ct);

        logger.LogInformation("Successfully deleted student {StudentId}", request.Id);

        return Result.Deleted;
    }
}
