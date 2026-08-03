using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;

public sealed class GetInstructorCoursesCountQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetInstructorCoursesCountQueryHandler> logger)
    : IRequestHandler<GetInstructorCoursesCountQuery, Result<CoursesCountDto>>
{
    public async Task<Result<CoursesCountDto>> Handle(GetInstructorCoursesCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving courses count for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await mongoContext.Users
            .Find(u => u.Id == request.InstructorId && u is Instructor)
            .AnyAsync(ct);

        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var count = (int)await mongoContext.Courses
            .CountDocumentsAsync(course => course.InstructorId == request.InstructorId, cancellationToken: ct);

        logger.LogInformation("Successfully retrieved courses count for instructor {InstructorId}: {Count}", request.InstructorId, count);

        return new CoursesCountDto(count);
    }
}
