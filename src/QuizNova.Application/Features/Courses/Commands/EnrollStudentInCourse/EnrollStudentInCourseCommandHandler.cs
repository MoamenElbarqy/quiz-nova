using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.EnrollStudentInCourse;

public sealed class EnrollStudentInCourseCommandHandler(
    IAppDbContext dbContext,
    ILogger<EnrollStudentInCourseCommandHandler> logger)
    : IRequestHandler<EnrollStudentInCourseCommand, Result<Created>>
{
    public async Task<Result<Created>> Handle(EnrollStudentInCourseCommand request, CancellationToken ct)
    {
        logger.LogInformation("Enrolling student {StudentId} in course {CourseId}", request.StudentId,
            request.CourseId);

        if (await dbContext.StudentCourses.AnyAsync(sc => sc.Id == request.Id, ct))
        {
            logger.LogWarning("Enrollment failed: Enrollment ID {EnrollmentId} already exists", request.Id);
            return ApplicationErrors.StudentCourseEnrollmentIdAlreadyExists(request.Id);
        }

        var course = await dbContext.Courses
            .Include(c => c.StudentCourses)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, ct);

        if (course is null)
        {
            logger.LogWarning("Enrollment failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        var student = await dbContext.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, ct);

        if (student is null)
        {
            logger.LogWarning("Enrollment failed: Student {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var enrollmentResult = course.Enroll(student, request.Id);

        if (enrollmentResult.IsError)
        {
            logger.LogWarning("Enrollment failed: {ErrorDescription}", enrollmentResult.TopError.Description);
            return enrollmentResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully enrolled student {StudentId} in course {CourseId}", request.StudentId,
            request.CourseId);

        return Result.Created;
    }
}
