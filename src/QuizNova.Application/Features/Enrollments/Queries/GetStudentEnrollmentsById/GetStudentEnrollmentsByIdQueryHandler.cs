using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsById;

public sealed class GetStudentEnrollmentsByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentEnrollmentsByIdQueryHandler> logger)
    : IRequestHandler<GetStudentEnrollmentsByIdQuery, Result<List<EnrollmentDto>>>
{
    public async Task<Result<List<EnrollmentDto>>> Handle(GetStudentEnrollmentsByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrollments for student with ID: {StudentId}", request.StudentId);

        var studentExists = await dbContext.Students.AsNoTracking().AnyAsync(s => s.Id == request.StudentId, ct);
        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var enrollments = await dbContext.Enrollments
            .Where(sc => sc.StudentId == request.StudentId)
            .AsNoTracking()
            .Select(sc => new EnrollmentDto(
                sc.Id,
                sc.CourseId,
                sc.Course!.Name,
                new EnrollmentInstructorDto(
                    sc.Course.InstructorId ?? Guid.Empty,
                    sc.Course.Instructor!.PersonalInformation.Name),
                new EnrollmentStudentDto(
                    sc.StudentId,
                    sc.Student!.PersonalInformation.Name,
                    dbContext.QuizAttempts.Count(qa =>
                        qa.StudentId == sc.StudentId && qa.Quiz!.CourseId == sc.CourseId)),
                sc.EnrolledOnUtc))
            .ToListAsync(ct);

        return enrollments;
    }
}
