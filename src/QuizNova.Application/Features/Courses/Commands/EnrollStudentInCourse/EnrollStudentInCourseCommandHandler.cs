using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.StudentCourses;

namespace QuizNova.Application.Features.Courses.Commands.EnrollStudentInCourse;

public sealed class EnrollStudentInCourseCommandHandler(
    IAppDbContext dbContext,
    ILogger<EnrollStudentInCourseCommandHandler> logger)
    : IRequestHandler<EnrollStudentInCourseCommand, Result<Created>>
{
    public async Task<Result<Created>> Handle(EnrollStudentInCourseCommand request, CancellationToken ct)
    {
        logger.LogInformation("Enrolling student {StudentId} in course {CourseId}", request.StudentId, request.CourseId);

        if (!await dbContext.Courses.AnyAsync(course => course.Id == request.CourseId, ct))
        {
            logger.LogWarning("Enrollment failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        if (!await dbContext.Students.AnyAsync(student => student.Id == request.StudentId, ct))
        {
            logger.LogWarning("Enrollment failed: Student {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        if (await dbContext.StudentCourses.AnyAsync(
                studentCourse =>
                    studentCourse.CourseId == request.CourseId &&
                    studentCourse.StudentId == request.StudentId,
                ct))
        {
            logger.LogWarning(
                "Enrollment failed: Student {StudentId} already enrolled in course {CourseId}",
                request.StudentId,
                request.CourseId);
            return ApplicationErrors.StudentAlreadyEnrolledInCourse(request.StudentId, request.CourseId);
        }

        var createStudentCourseResult = StudentCourse.Create(
            Guid.NewGuid(),
            request.StudentId,
            request.CourseId,
            DateTimeOffset.UtcNow);

        if (createStudentCourseResult.IsError)
        {
            logger.LogWarning("Enrollment failed: {ErrorDescription}", createStudentCourseResult.TopError.Description);
            return createStudentCourseResult.TopError;
        }

        await dbContext.StudentCourses.AddAsync(createStudentCourseResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully enrolled student {StudentId} in course {CourseId}", request.StudentId, request.CourseId);

        return Result.Created;
    }
}
