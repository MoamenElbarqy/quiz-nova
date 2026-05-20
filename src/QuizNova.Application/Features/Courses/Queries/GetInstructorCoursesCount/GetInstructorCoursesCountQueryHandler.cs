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

        var countInfo = await dbContext.Instructors
            .Where(instructor => instructor.Id == request.InstructorId)
            .Select(instructor => new { Count = instructor.Courses.Count() })
            .FirstOrDefaultAsync(ct);

        if (countInfo is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        logger.LogInformation("Successfully retrieved courses count for instructor {InstructorId}: {Count}", request.InstructorId, countInfo.Count);

        return new CoursesCountDto(countInfo.Count);
    }
}

