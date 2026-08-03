using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetCourseByIdQueryHandler> logger)
    : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving course with ID: {CourseId}", request.CourseId);

        var course = await mongoContext.Courses
            .Find(c => c.Id == request.CourseId)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Course with ID {CourseId} was not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        var instructorName = string.Empty;
        if (course.InstructorId.HasValue)
        {
            var instructor = await mongoContext.Users
                .Find(u => u.Id == course.InstructorId.Value && u is Instructor)
                .FirstOrDefaultAsync(ct) as Instructor;
            instructorName = instructor?.PersonalInformation.Name ?? string.Empty;
        }

        var enrolledStudentsCount = (int)await mongoContext.Enrollments
            .CountDocumentsAsync(sc => sc.CourseId == request.CourseId, cancellationToken: ct);

        var stats = await mongoContext.Quizzes
            .Aggregate()
            .Match(q => q.CourseId == request.CourseId)
            .Group(
                _ => 1,
                g => new
                {
                    Count = g.Count(),
                    TotalMarks = g.Sum(q => q.Questions.Sum(question => question.Marks)),
                })
            .FirstOrDefaultAsync(ct);

        var quizzesCount = stats?.Count ?? 0;
        var consumedMarks = stats?.TotalMarks ?? 0;

        var response = new CourseDto(
            course.Id,
            course.Name,
            course.InstructorId,
            instructorName,
            enrolledStudentsCount,
            quizzesCount,
            course.MaximumMarks - consumedMarks);

        logger.LogInformation("Successfully retrieved course with ID: {CourseId}", request.CourseId);

        return response;
    }
}
