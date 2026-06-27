using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Commands.RemoveStudentFromCourse;

public sealed class RemoveStudentFromCourseCommandHandler(
    IAppDbContext dbContext,
    ILogger<RemoveStudentFromCourseCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<RemoveStudentFromCourseCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(RemoveStudentFromCourseCommand request, CancellationToken ct)
    {
        logger.LogInformation("Removing enrollment {EnrollmentId} for student {StudentId}", request.EnrollmentId,
            request.StudentId);

        var enrollment = await dbContext.Enrollments
            .FirstOrDefaultAsync(
                entity => entity.Id == request.EnrollmentId && entity.StudentId == request.StudentId,
                ct);

        if (enrollment is null)
        {
            logger.LogWarning(
                "Remove enrollment failed: Enrollment {EnrollmentId} for student {StudentId} was not found",
                request.EnrollmentId,
                request.StudentId);
            return ApplicationErrors.EnrollmentNotFound(request.EnrollmentId);
        }

        var deleteResult = enrollment.Delete();
        if (deleteResult.IsError)
        {
            logger.LogWarning("Remove enrollment failed: {ErrorDescription}", deleteResult.TopError.Description);
            return deleteResult.TopError;
        }

        dbContext.Enrollments.Remove(enrollment);
        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["courses", "students"], ct);

        logger.LogInformation("Successfully removed enrollment {EnrollmentId} for student {StudentId}", request.EnrollmentId,
            request.StudentId);

        return Result.Deleted;
    }
}
