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
    IMongoDbContext mongoContext,
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

        var enrollmentsList = await dbContext.Enrollments
            .Where(sc => sc.StudentId == request.StudentId)
            .AsNoTracking()
            .Select(sc => new
            {
                sc.Id,
                sc.CourseId,
                CourseName = sc.Course!.Name,
                InstructorId = sc.Course.InstructorId ?? Guid.Empty,
                InstructorName = sc.Course.Instructor != null
                    ? sc.Course.Instructor.PersonalInformation.Name
                    : string.Empty,
                sc.StudentId,
                StudentName = sc.Student!.PersonalInformation.Name,
                sc.EnrolledOnUtc,
            })
            .ToListAsync(ct);

        var courseIds = enrollmentsList.Select(e => e.CourseId).ToList();

        var quizzesTask = mongoContext.Quizzes
            .Find(q => courseIds.Contains(q.CourseId))
            .Project(q => new { q.Id, q.CourseId })
            .ToListAsync(ct);

        var attemptsTask = mongoContext.QuizAttempts
            .Find(a => a.StudentId == request.StudentId)
            .Project(a => new { a.QuizId })
            .ToListAsync(ct);

        await Task.WhenAll(quizzesTask, attemptsTask);

        var quizzes = quizzesTask.Result;
        var quizCourseMap = quizzes.ToDictionary(q => q.Id, q => q.CourseId);

        var attemptCountsByCourse = attemptsTask.Result
            .GroupBy(a => quizCourseMap.GetValueOrDefault(a.QuizId))
            .ToDictionary(g => g.Key, g => g.Count());

        var enrollments = enrollmentsList.Select(sc => new EnrollmentDto(
            sc.Id,
            sc.CourseId,
            sc.CourseName,
            new EnrollmentInstructorDto(sc.InstructorId, sc.InstructorName),
            new EnrollmentStudentDto(
                sc.StudentId,
                sc.StudentName,
                attemptCountsByCourse.GetValueOrDefault(sc.CourseId, 0)),
            sc.EnrolledOnUtc)).ToList();

        return enrollments;
    }
}

