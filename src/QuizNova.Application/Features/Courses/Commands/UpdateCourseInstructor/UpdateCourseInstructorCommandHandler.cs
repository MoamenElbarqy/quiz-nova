using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;

public sealed class UpdateCourseInstructorCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<UpdateCourseInstructorCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateCourseInstructorCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(UpdateCourseInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor for course {CourseId}", request.CourseId);

        var course = await mongoContext.Courses
            .Find(entity => entity.Id == request.CourseId)
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            logger.LogWarning("Course instructor update failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        if (request.InstructorId.HasValue)
        {
            var instructorExists = await mongoContext.Users
                .Find(u => u.Id == request.InstructorId.Value && u is Instructor)
                .AnyAsync(ct);

            if (!instructorExists)
            {
                logger.LogWarning("Course instructor update failed: Instructor {InstructorId} not found",
                    request.InstructorId);
                return ApplicationErrors.InstructorNotFound(request.InstructorId.Value);
            }
        }

        var updateResult = course.UpdateInstructor(request.InstructorId);

        if (updateResult.IsError)
        {
            logger.LogWarning("Course instructor update failed: {ErrorDescription}", updateResult.TopError.Description);
            return updateResult.TopError;
        }

        await mongoContext.Courses.ReplaceOneAsync(c => c.Id == course.Id, course, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Courses], ct);

        var instructorName = string.Empty;
        if (request.InstructorId.HasValue)
        {
            var instructor = await mongoContext.Users
                .Find(u => u.Id == request.InstructorId.Value && u is Instructor)
                .FirstOrDefaultAsync(ct) as Instructor;
            instructorName = instructor?.PersonalInformation.Name ?? string.Empty;
        }

        var enrolledStudentsCount = (int)await mongoContext.Enrollments
            .CountDocumentsAsync(e => e.CourseId == course.Id, cancellationToken: ct);

        var courseQuizzes = await mongoContext.Quizzes
            .Find(q => q.CourseId == course.Id)
            .ToListAsync(ct);

        var quizzesCount = courseQuizzes.Count;
        var consumedMarks = courseQuizzes.Sum(q => q.Questions.Sum(question => question.Marks));

        logger.LogInformation("Successfully updated instructor for course {CourseId}", request.CourseId);

        return new CourseDto(
            course.Id,
            course.Name,
            course.InstructorId,
            instructorName,
            enrolledStudentsCount,
            quizzesCount,
            course.MaximumMarks - consumedMarks);
    }
}

