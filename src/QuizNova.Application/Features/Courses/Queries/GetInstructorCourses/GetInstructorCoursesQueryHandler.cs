using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;

public sealed class GetInstructorCoursesQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetInstructorCoursesQueryHandler> logger)
    : IRequestHandler<GetInstructorCoursesQuery, Result<List<CourseDto>>>
{
    public async Task<Result<List<CourseDto>>> Handle(GetInstructorCoursesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving courses for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await mongoContext.Users
            .Find(u => u.Id == request.InstructorId && u is Instructor)
            .AnyAsync(ct);

        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var instructor = await mongoContext.Users
            .Find(u => u.Id == request.InstructorId && u is Instructor)
            .FirstOrDefaultAsync(ct) as Instructor;

        var instructorName = instructor?.PersonalInformation.Name ?? string.Empty;

        var courses = await mongoContext.Courses
            .Find(c => c.InstructorId == request.InstructorId)
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
                instructorName,
                0,
                quizzesCount,
                remainingMarks);
        }).ToList();

        logger.LogInformation("Successfully retrieved {Count} courses for instructor {InstructorId}",
            instructorCourses.Count, request.InstructorId);

        return instructorCourses;
    }
}
