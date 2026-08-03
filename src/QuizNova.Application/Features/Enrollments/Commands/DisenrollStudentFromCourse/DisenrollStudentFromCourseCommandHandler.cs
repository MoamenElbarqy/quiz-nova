using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Events;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Enrollments.Commands.DisenrollStudentFromCourse;

public sealed class DisenrollStudentFromCourseCommandHandler(
    IMongoDbContext mongoContext,
    IDomainEventTracker eventTracker,
    ILogger<DisenrollStudentFromCourseCommandHandler> logger,
    ICacheInvalidator cacheInvalidator,
    IUser currentUser)
    : IRequestHandler<DisenrollStudentFromCourseCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DisenrollStudentFromCourseCommand request, CancellationToken ct)
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

        logger.LogInformation("Disenrolling student {StudentId} from enrollment {EnrollmentId}", request.StudentId,
            request.EnrollmentId);

        var enrollment = await mongoContext.Enrollments
            .Find(
                entity => entity.Id == request.EnrollmentId && entity.StudentId == request.StudentId)
            .FirstOrDefaultAsync(ct);

        if (enrollment is null)
        {
            logger.LogWarning(
                "Disenrollment failed: Enrollment {EnrollmentId} for student {StudentId} was not found",
                request.EnrollmentId,
                request.StudentId);
            return ApplicationErrors.EnrollmentNotFound(request.EnrollmentId);
        }

        var course = await mongoContext.Courses
            .Find(c => c.Id == enrollment.CourseId)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Disenrollment failed: Course {CourseId} not found", enrollment.CourseId);
            return ApplicationErrors.CourseNotFound(enrollment.CourseId);
        }

        var student = await mongoContext.Users
            .Find(u => u.Id == request.StudentId && u is Student)
            .FirstOrDefaultAsync(ct) as Student;

        if (student is null)
        {
            logger.LogWarning("Disenrollment failed: Student {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var disenrollResult = course.Disenroll(student);
        if (disenrollResult.IsError)
        {
            logger.LogWarning("Disenrollment failed: {ErrorDescription}", disenrollResult.TopError.Description);
            return disenrollResult.TopError;
        }

        await mongoContext.Courses.ReplaceOneAsync(c => c.Id == course.Id, course, cancellationToken: ct);
        await mongoContext.Enrollments.DeleteOneAsync(e => e.Id == enrollment.Id, cancellationToken: ct);

        eventTracker.TrackEntity(course);
        await cacheInvalidator.InvalidateAsync([CacheTags.Courses, CacheTags.Students], ct);

        logger.LogInformation("Successfully disenrolled student {StudentId} from enrollment {EnrollmentId}", request.StudentId,
            request.EnrollmentId);

        return Result.Deleted;
    }
}
