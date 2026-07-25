using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;

public sealed class UpdateCourseInstructorCommandHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<UpdateCourseInstructorCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateCourseInstructorCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(UpdateCourseInstructorCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating instructor for course {CourseId}", request.CourseId);

        var course = await dbContext.Courses
            .FirstOrDefaultAsync(entity => entity.Id == request.CourseId, ct);

        if (course is null)
        {
            logger.LogWarning("Course instructor update failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.CourseNotFound(request.CourseId);
        }

        if (request.InstructorId.HasValue &&
            !await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId.Value, ct))
        {
            logger.LogWarning("Course instructor update failed: Instructor {InstructorId} not found",
                request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId.Value);
        }

        var updateResult = course.UpdateInstructor(request.InstructorId);

        if (updateResult.IsError)
        {
            logger.LogWarning("Course instructor update failed: {ErrorDescription}", updateResult.TopError.Description);
            return updateResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Courses], ct);

        var instructorName = request.InstructorId.HasValue
            ? await dbContext.Instructors
                .Where(instructor => instructor.Id == request.InstructorId.Value)
                .Select(instructor => instructor.PersonalInformation.Name)
                .FirstOrDefaultAsync(ct) ?? string.Empty
            : string.Empty;

        var enrolledStudentsCount = await dbContext.Enrollments
            .CountAsync(enrollment => enrollment.CourseId == course.Id, ct);

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

