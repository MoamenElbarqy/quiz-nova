using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetCourseByIdQueryHandler> logger)
    : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving course with ID: {CourseId}", request.CourseId);

        var course = await dbContext.Courses
            .AsNoTracking()
            .Where(c => c.Id == request.CourseId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.InstructorId,
                c.MaximumMarks,
                InstructorName = dbContext.Instructors
                    .Where(i => i.Id == c.InstructorId)
                    .Select(i => i.PersonalInformation.Name)
                    .FirstOrDefault() ?? string.Empty,
                EnrolledStudentsCount = dbContext.Enrollments.Count(sc => sc.CourseId == c.Id),
            })
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Course with ID {CourseId} was not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

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
            course.InstructorName,
            course.EnrolledStudentsCount,
            quizzesCount,
            course.MaximumMarks - consumedMarks);

        logger.LogInformation("Successfully retrieved course with ID: {CourseId}", request.CourseId);

        return response;
    }
}

