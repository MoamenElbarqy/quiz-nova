using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.RemoveStudentFromCourse;

public sealed class RemoveStudentFromCourseCommandHandler(
    IAppDbContext dbContext,
    ILogger<RemoveStudentFromCourseCommandHandler> logger)
    : IRequestHandler<RemoveStudentFromCourseCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(RemoveStudentFromCourseCommand request, CancellationToken ct)
    {
        logger.LogInformation("Removing student {StudentId} from course {CourseId}", request.StudentId, request.CourseId);

        var enrollment = await dbContext.Enrollments
            .FirstOrDefaultAsync(
                entity => entity.CourseId == request.CourseId && entity.StudentId == request.StudentId,
                ct);

        if (enrollment is null)
        {
            logger.LogWarning(
                "Remove enrollment failed: Student {StudentId} is not enrolled in course {CourseId}",
                request.StudentId,
                request.CourseId);
            return ApplicationErrors.StudentNotEnrolledInCourse(request.StudentId, request.CourseId);
        }

        var deleteResult = enrollment.Delete();
        if (deleteResult.IsError)
        {
            logger.LogWarning("Remove enrollment failed: {ErrorDescription}", deleteResult.TopError.Description);
            return deleteResult.TopError;
        }

        dbContext.Enrollments.Remove(enrollment);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully removed student {StudentId} from course {CourseId}", request.StudentId, request.CourseId);

        return Result.Deleted;
    }
}
