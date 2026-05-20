using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesById;

public sealed class GetInstructorCoursesByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetInstructorCoursesByIdQueryHandler> logger)
    : IRequestHandler<GetInstructorCoursesByIdQuery, Result<List<CourseDto>>>
{
    public async Task<Result<List<CourseDto>>> Handle(GetInstructorCoursesByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving courses for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await dbContext.Instructors.AnyAsync(i => i.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var instructorCourses = await dbContext.Courses
            .Where(c => c.InstructorId == request.InstructorId)
            .AsNoTracking()
            .Select(course => new CourseDto(
                course.Id,
                course.Name,
                course.InstructorId,
                course.Instructor != null ? course.Instructor.PersonalInformation.Name : string.Empty,
                course.Enrollments.Count(),
                course.Quizzes.Count(),
                course.MaximumMarks - course.Quizzes
                    .SelectMany(quiz => quiz.Questions)
                    .Sum(q => q.Marks)))
            .ToListAsync(ct);

        if (instructorCourses.Count == 0)
        {
            logger.LogInformation("No courses found for instructor {InstructorId}", request.InstructorId);
            return ApplicationErrors.NoCoursesForInstructor(request.InstructorId);
        }

        logger.LogInformation("Successfully retrieved {Count} courses for instructor {InstructorId}",
            instructorCourses.Count, request.InstructorId);

        return instructorCourses;
    }
}
