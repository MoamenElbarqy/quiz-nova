using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetAllCoursesEnrollmentCount;

public sealed class GetAllCoursesEnrollmentCountQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAllCoursesEnrollmentCountQueryHandler> logger)
    : IRequestHandler<GetAllCoursesEnrollmentCountQuery, Result<List<CourseEnrollmentCountDto>>>
{
    public async Task<Result<List<CourseEnrollmentCountDto>>> Handle(
        GetAllCoursesEnrollmentCountQuery request,
        CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrollment counts for all courses");

        var courses = await mongoContext.Courses
            .Find(_ => true)
            .ToListAsync(ct);

        var enrollments = await mongoContext.Enrollments
            .Find(_ => true)
            .ToListAsync(ct);

        var enrollmentCountsByCourse = enrollments
            .GroupBy(e => e.CourseId)
            .ToDictionary(g => g.Key, g => g.Count());

        var enrollmentCounts = courses
            .Select(course => new CourseEnrollmentCountDto(
                course.Id,
                course.Name,
                enrollmentCountsByCourse.GetValueOrDefault(course.Id, 0)))
            .ToList();

        enrollmentCounts = [.. enrollmentCounts.OrderByDescending(c => c.EnrollmentsCount)];

        logger.LogInformation("Successfully retrieved enrollment counts for {Count} courses", enrollmentCounts.Count);

        return enrollmentCounts;
    }
}
