using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;

public sealed class GetInstructorCoursesPerformanceQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetInstructorCoursesPerformanceQueryHandler> logger)
    : IRequestHandler<GetInstructorCoursesPerformanceQuery, Result<List<CoursePerformanceDto>>>
{
    public async Task<Result<List<CoursePerformanceDto>>> Handle(
        GetInstructorCoursesPerformanceQuery request,
        CancellationToken ct)
    {
        logger.LogInformation("Retrieving course performance for instructor with ID: {InstructorId}",
            request.InstructorId);

        var instructor = await mongoContext.Users
            .Find(u => u.Id == request.InstructorId && u is Instructor)
            .FirstOrDefaultAsync(ct) as Instructor;

        if (instructor is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var instructorName = instructor.PersonalInformation.Name;

        var courses = await mongoContext.Courses
            .Find(c => c.InstructorId == request.InstructorId)
            .ToListAsync(ct);

        var courseIds = courses.Select(c => c.Id).ToList();

        var quizzes = await mongoContext.Quizzes
            .Find(q => courseIds.Contains(q.CourseId))
            .ToListAsync(ct);

        var quizIds = quizzes.Select(q => q.Id).ToList();

        var attempts = await mongoContext.QuizAttempts
            .Find(qa => quizIds.Contains(qa.QuizId))
            .ToListAsync(ct);

        var performanceList = courses.Select(course =>
        {
            var courseQuizIds = quizzes.Where(q => q.CourseId == course.Id).Select(q => q.Id).ToHashSet();
            var courseAttempts = attempts.Where(a => courseQuizIds.Contains(a.QuizId)).ToList();

            var avgScore = courseAttempts.Count > 0
                ? courseAttempts.Average(a => a.Score)
                : 0.0;

            return new CoursePerformanceDto(
                course.Id,
                course.Name,
                instructorName,
                course.EnrollmentsCount,
                avgScore);
        }).ToList();

        logger.LogInformation("Successfully retrieved performance data for {Count} courses", performanceList.Count);

        return performanceList;
    }
}
