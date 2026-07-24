using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;

public sealed class GetInstructorCoursesPerformanceQueryHandler(
    IAppDbContext dbContext,
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

        var instructorExists = await dbContext.Instructors.AnyAsync(i => i.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var courses = await dbContext.Courses
            .Where(c => c.InstructorId == request.InstructorId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                InstructorName = c.Instructor != null ? c.Instructor.PersonalInformation.Name : string.Empty,
                StudentsCount = c.Enrollments.Count(),
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var courseIds = courses.Select(c => c.Id).ToList();

        var quizzes = await mongoContext.Quizzes
            .Find(q => courseIds.Contains(q.CourseId))
            .ToListAsync(ct);

        var quizMap = quizzes.ToDictionary(q => q.Id);
        var quizIds = quizMap.Keys.ToList();

        var attempts = await mongoContext.QuizAttempts
            .Find(qa => quizIds.Contains(qa.QuizId))
            .ToListAsync(ct);

        foreach (var attempt in attempts)
        {
            if (quizMap.TryGetValue(attempt.QuizId, out var quiz))
            {
                attempt.AttachQuizQuestions(quiz.Questions);
            }
        }

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
                course.InstructorName,
                course.StudentsCount,
                avgScore);
        }).ToList();

        logger.LogInformation("Successfully retrieved performance data for {Count} courses", performanceList.Count);

        return performanceList;
    }
}
