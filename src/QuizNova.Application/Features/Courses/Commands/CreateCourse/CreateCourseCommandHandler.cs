using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(
    IAppDbContext dbContext,
    ILogger<CreateCourseCommandHandler> logger)
    : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating course {CourseId}", request.Id);

        if (await dbContext.Courses.AnyAsync(course => course.Id == request.Id, ct))
        {
            logger.LogWarning("Course creation failed: Course ID {CourseId} already exists", request.Id);
            return ApplicationErrors.CourseIdAlreadyExists(request.Id);
        }

        if (request.InstructorId.HasValue &&
            !await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId.Value, ct))
        {
            logger.LogWarning("Course creation failed: Instructor {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId.Value);
        }

        var createCourseResult = Course.Create(
            request.Id,
            request.InstructorId,
            request.Name,
            request.MinimumPassingMarks,
            request.MaximumMarks,
            []);

        if (createCourseResult.IsError)
        {
            logger.LogWarning("Course creation failed: {ErrorDescription}", createCourseResult.TopError.Description);
            return createCourseResult.TopError;
        }

        await dbContext.Courses.AddAsync(createCourseResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        var instructorName = request.InstructorId.HasValue
            ? await dbContext.Instructors
                .Where(instructor => instructor.Id == request.InstructorId.Value)
                .Select(instructor => instructor.PersonalInformation.Name)
                .FirstOrDefaultAsync(ct) ?? string.Empty
            : string.Empty;

        logger.LogInformation("Successfully created course {CourseId}", request.Id);

        return new CourseDto(
            createCourseResult.Value.Id,
            createCourseResult.Value.Name,
            createCourseResult.Value.InstructorId,
            instructorName,
            EnrolledStudentsCount: 0,
            QuizzesCount: 0);
    }
}
