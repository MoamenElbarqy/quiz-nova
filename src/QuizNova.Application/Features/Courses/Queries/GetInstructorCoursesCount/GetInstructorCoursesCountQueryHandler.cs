using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;

public sealed class GetInstructorCoursesCountQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetInstructorCoursesCountQueryHandler> logger)
    : IRequestHandler<GetInstructorCoursesCountQuery, Result<CoursesCountDto>>
{
    public async Task<Result<CoursesCountDto>> Handle(GetInstructorCoursesCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving courses count for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var courseCount = await dbContext.Courses.CountAsync(course => course.InstructorId == request.InstructorId, ct);

        logger.LogInformation("Successfully retrieved courses count for instructor {InstructorId}: {Count}", request.InstructorId, courseCount);

        return new CoursesCountDto(courseCount);
    }
}

