using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;

public sealed class GetStudentEnrollmentsCountQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetStudentEnrollmentsCountQueryHandler> logger)
    : IRequestHandler<GetStudentEnrollmentsCountQuery, Result<EnrollmentCountDto>>
{
    public async Task<Result<EnrollmentCountDto>> Handle(GetStudentEnrollmentsCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrolled courses count for student with ID: {StudentId}", request.StudentId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == request.StudentId)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var courseCount = (int)await mongoContext.Enrollments
            .CountDocumentsAsync(e => e.StudentId == request.StudentId, cancellationToken: ct);

        logger.LogInformation("Successfully retrieved enrolled courses count for student {StudentId}: {Count}",
            request.StudentId, courseCount);

        return new EnrollmentCountDto(courseCount);
    }
}
