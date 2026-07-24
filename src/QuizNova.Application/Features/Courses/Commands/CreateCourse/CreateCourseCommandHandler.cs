using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(
    IAppDbContext dbContext,
    ILogger<CreateCourseCommandHandler> logger,
    ICacheInvalidator cacheInvalidator,
    IUser currentUser)
    : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var adminId))
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var admin = await dbContext.Admins.Where(a => a.Id == adminId).FirstOrDefaultAsync(ct);
        if (admin is null)
        {
            return ApplicationErrors.AdminNotFound(adminId);
        }

        if (request.InstructorId.HasValue &&
            !await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId.Value, ct))
        {
            logger.LogWarning("Course creation failed: Instructor {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId.Value);
        }

        var createCourseResult = Course.Create(
            request.InstructorId,
            request.Name,
            request.MinimumPassingMarks,
            request.MaximumMarks,
            [],
            []);

        if (createCourseResult.IsError)
        {
            logger.LogWarning("Course creation failed: {ErrorDescription}", createCourseResult.TopError.Description);
            return createCourseResult.TopError;
        }

        await dbContext.Courses.AddAsync(createCourseResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["courses"], ct);

        logger.LogInformation("Successfully created course {CourseId}", createCourseResult.Value.Id);

        var instructorName = request.InstructorId.HasValue
            ? await dbContext.Instructors
                .Where(i => i.Id == request.InstructorId.Value)
                .Select(i => i.PersonalInformation.Name)
                .FirstOrDefaultAsync(ct)
            : null;

        return createCourseResult.Value.ToCourseDto(instructorName, 0, 0, createCourseResult.Value.MaximumMarks);
    }
}
