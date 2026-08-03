using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Events;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(
    IMongoDbContext mongoContext,
    IDomainEventTracker eventTracker,
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

        var admin = await mongoContext.Users
            .Find(u => u.Id == adminId && u is Admin)
            .FirstOrDefaultAsync(ct) as Admin;

        if (admin is null)
        {
            return ApplicationErrors.AdminNotFound(adminId);
        }

        Instructor? instructor = null;
        if (request.InstructorId.HasValue)
        {
            instructor = await mongoContext.Users
                .Find(u => u.Id == request.InstructorId.Value && u is Instructor)
                .FirstOrDefaultAsync(ct) as Instructor;

            if (instructor is null)
            {
                logger.LogWarning("Course creation failed: Instructor {InstructorId} not found", request.InstructorId);
                return ApplicationErrors.InstructorNotFound(request.InstructorId.Value);
            }
        }

        var createCourseResult = Course.Create(
            request.InstructorId,
            request.Name,
            request.MinimumPassingMarks,
            request.MaximumMarks);

        if (createCourseResult.IsError)
        {
            logger.LogWarning("Course creation failed: {ErrorDescription}", createCourseResult.TopError.Description);
            return createCourseResult.TopError;
        }

        await mongoContext.Courses.InsertOneAsync(createCourseResult.Value, cancellationToken: ct);
        eventTracker.TrackEntity(createCourseResult.Value);
        await cacheInvalidator.InvalidateAsync([CacheTags.Courses], ct);

        logger.LogInformation("Successfully created course {CourseId}", createCourseResult.Value.Id);

        var instructorName = instructor?.PersonalInformation.Name ?? string.Empty;

        return createCourseResult.Value.ToCourseDto(instructorName, 0, 0, createCourseResult.Value.MaximumMarks);
    }
}
