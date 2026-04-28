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
                dbContext.Instructors
                    .Where(instructor => instructor.Id == course.InstructorId)
                    .Select(instructor => instructor.PersonalInformation.Name)
                    .FirstOrDefault() ?? string.Empty,
                dbContext.StudentCourses.Count(studentCourse => studentCourse.CourseId == course.Id),
                dbContext.Quizzes.Count(quiz => quiz.CourseId == course.Id)))
            .ToListAsync(ct);

        if (!instructorCourses.Any())
        {
            logger.LogInformation("No courses found for instructor {InstructorId}", request.InstructorId);
            return ApplicationErrors.NoCoursesForInstructor(request.InstructorId);
        }

        logger.LogInformation("Successfully retrieved {Count} courses for instructor {InstructorId}", instructorCourses.Count, request.InstructorId);

        return instructorCourses;
    }
}


