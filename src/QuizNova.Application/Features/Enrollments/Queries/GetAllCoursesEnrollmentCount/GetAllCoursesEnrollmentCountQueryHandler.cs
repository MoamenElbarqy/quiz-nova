using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetAllCoursesEnrollmentCount;

public sealed class GetAllCoursesEnrollmentCountQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetAllCoursesEnrollmentCountQueryHandler> logger)
    : IRequestHandler<GetAllCoursesEnrollmentCountQuery, Result<List<CourseEnrollmentCountDto>>>
{
    public async Task<Result<List<CourseEnrollmentCountDto>>> Handle(
        GetAllCoursesEnrollmentCountQuery request,
        CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrollment counts for all courses");

        var enrollmentCounts = await dbContext.Courses
            .AsNoTracking()
            .Select(course => new CourseEnrollmentCountDto(
                course.Id,
                course.Name,
                dbContext.Enrollments.Count(enrollment => enrollment.CourseId == course.Id)))
            .ToListAsync(ct);

        enrollmentCounts = [.. enrollmentCounts.OrderByDescending(c => c.EnrollmentsCount)];

        logger.LogInformation("Successfully retrieved enrollment counts for {Count} courses", enrollmentCounts.Count);

        return enrollmentCounts;
    }
}
