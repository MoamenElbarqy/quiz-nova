using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;

public sealed class GetInstructorCoursesQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetInstructorCoursesQueryHandler> logger)
    : IRequestHandler<GetInstructorCoursesQuery, Result<List<CourseDto>>>
{
    public async Task<Result<List<CourseDto>>> Handle(GetInstructorCoursesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving courses for instructor with ID: {InstructorId}", request.InstructorId);

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
                c.InstructorId,
                InstructorName = c.Instructor != null ? c.Instructor.PersonalInformation.Name : string.Empty,
                StudentsCount = c.Enrollments.Count(),
                c.MaximumMarks,
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var courseIds = courses.Select(c => c.Id).ToList();

        var quizzes = await mongoContext.Quizzes
            .Find(q => courseIds.Contains(q.CourseId))
            .ToListAsync(ct);

        var instructorCourses = courses.Select(c =>
        {
            var courseQuizzes = quizzes.Where(q => q.CourseId == c.Id).ToList();
            var quizzesCount = courseQuizzes.Count;
            var allocatedMarks = courseQuizzes.Sum(q => q.Questions.Sum(question => question.Marks));
            var remainingMarks = c.MaximumMarks - allocatedMarks;

            return new CourseDto(
                c.Id,
                c.Name,
                c.InstructorId,
                c.InstructorName,
                c.StudentsCount,
                quizzesCount,
                remainingMarks);
        }).ToList();

        logger.LogInformation("Successfully retrieved {Count} courses for instructor {InstructorId}",
            instructorCourses.Count, request.InstructorId);

        return instructorCourses;
    }
}
