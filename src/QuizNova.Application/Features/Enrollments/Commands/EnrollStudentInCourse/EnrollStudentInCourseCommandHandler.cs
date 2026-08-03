using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Events;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;

public sealed class EnrollStudentInCourseCommandHandler(
    IMongoDbContext mongoContext,
    IDomainEventTracker eventTracker,
    ILogger<EnrollStudentInCourseCommandHandler> logger,
    ICacheInvalidator cacheInvalidator,
    IUser currentUser)
    : IRequestHandler<EnrollStudentInCourseCommand, Result<Created>>
{
    public async Task<Result<Created>> Handle(EnrollStudentInCourseCommand request, CancellationToken ct)
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

        logger.LogInformation("Enrolling student {StudentId} in course {CourseId}", request.StudentId,
            request.CourseId);

        var course = await mongoContext.Courses
            .Find(c => c.Id == request.CourseId)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Enrollment failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        var student = await mongoContext.Users
            .Find(u => u.Id == request.StudentId && u is Student)
            .FirstOrDefaultAsync(ct) as Student;

        if (student is null)
        {
            logger.LogWarning("Enrollment failed: Student {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var existingEnrollment = await mongoContext.Enrollments
            .Find(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId)
            .FirstOrDefaultAsync(ct);

        if (existingEnrollment is not null)
        {
            logger.LogWarning("Enrollment failed: Student {StudentId} is already enrolled in course {CourseId}", request.StudentId, request.CourseId);
            return CourseErrors.StudentAlreadyEnrolled(request.StudentId);
        }

        var enrollmentResult = course.Enroll(student);

        if (enrollmentResult.IsError)
        {
            logger.LogWarning("Enrollment failed: {ErrorDescription}", enrollmentResult.TopError.Description);
            return enrollmentResult.TopError;
        }

        await mongoContext.Courses.ReplaceOneAsync(c => c.Id == course.Id, course, cancellationToken: ct);
        await mongoContext.Enrollments.InsertOneAsync(enrollmentResult.Value, cancellationToken: ct);

        eventTracker.TrackEntity(course);
        await cacheInvalidator.InvalidateAsync([CacheTags.Courses, CacheTags.Students], ct);

        logger.LogInformation("Successfully enrolled student {StudentId} in course {CourseId}", request.StudentId,
            request.CourseId);

        return Result.Created;
    }
}
