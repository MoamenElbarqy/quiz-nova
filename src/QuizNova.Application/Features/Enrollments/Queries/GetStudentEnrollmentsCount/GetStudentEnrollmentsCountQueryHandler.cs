using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;

public sealed class GetStudentEnrollmentsCountQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentEnrollmentsCountQueryHandler> logger)
    : IRequestHandler<GetStudentEnrollmentsCountQuery, Result<EnrollmentCountDto>>
{
    public async Task<Result<EnrollmentCountDto>> Handle(GetStudentEnrollmentsCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrolled courses count for student with ID: {StudentId}", request.StudentId);

        var studentExists =
            await dbContext.Students.AsNoTracking().AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var courseCount = await dbContext.Enrollments
            .CountAsync(enrollment => enrollment.StudentId == request.StudentId, ct);

        logger.LogInformation("Successfully retrieved enrolled courses count for student {StudentId}: {Count}",
            request.StudentId, courseCount);

        return new EnrollmentCountDto(courseCount);
    }
}
