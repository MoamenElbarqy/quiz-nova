using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;

public sealed class GetInstructorCoursesPerformanceQueryHandler(
    IAppDbContext dbContext,
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

        var performanceData = await dbContext.Courses
            .Where(c => c.InstructorId == request.InstructorId)
            .Select(course => new
            {
                course.Id,
                course.Name,
                InstructorName = course.Instructor != null ? course.Instructor.PersonalInformation.Name : string.Empty,
                StudentsCount = course.Enrollments.Count(),
                Attempts = course.Quizzes
                    .SelectMany(q => q.QuizAttempts)
                    .Select(qa => new
                    {
                        AutoGradedScore = qa.StudentAnswers
                            .OfType<AutoGradedAnswer>()
                            .Where(a => a.IsCorrect && a.Question != null)
                            .Sum(a => a.Question!.Marks),
                        ManuallyGradedScore = qa.StudentAnswers
                            .OfType<ManuallyGradedAnswers>()
                            .Sum(a => a.Score ?? 0),
                    })
                    .ToList(),
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var performanceList = performanceData.Select(p => new CoursePerformanceDto(
            p.Id,
            p.Name,
            p.InstructorName,
            p.StudentsCount,
            p.Attempts.Any() ? p.Attempts.Average(a => a.AutoGradedScore + a.ManuallyGradedScore) : 0.0)).ToList();

        logger.LogInformation("Successfully retrieved performance data for {Count} courses", performanceList.Count);

        return performanceList;
    }
}
