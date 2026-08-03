using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsById;

public sealed class GetStudentEnrollmentsByIdQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetStudentEnrollmentsByIdQueryHandler> logger)
    : IRequestHandler<GetStudentEnrollmentsByIdQuery, Result<List<EnrollmentDto>>>
{
    public async Task<Result<List<EnrollmentDto>>> Handle(GetStudentEnrollmentsByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving enrollments for student with ID: {StudentId}", request.StudentId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == request.StudentId && u is Student)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var enrollmentsList = await mongoContext.Enrollments
            .Find(sc => sc.StudentId == request.StudentId)
            .ToListAsync(ct);

        var courseIds = enrollmentsList.Select(e => e.CourseId).ToList();

        var courses = await mongoContext.Courses
            .Find(c => courseIds.Contains(c.Id))
            .ToListAsync(ct);
        var courseMap = courses.ToDictionary(c => c.Id);

        var student = await mongoContext.Users
            .Find(u => u.Id == request.StudentId && u is Student)
            .FirstOrDefaultAsync(ct) as Student;
        var studentName = student?.PersonalInformation.Name ?? string.Empty;

        var instructorIds = courses
            .Select(c => c.InstructorId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var instructorNames = new Dictionary<Guid, string>();
        if (instructorIds.Count != 0)
        {
            var instructors = await mongoContext.Users
                .Find(u => instructorIds.Contains(u.Id) && u is Instructor)
                .ToListAsync(ct);
            instructorNames = instructors
                .Cast<Instructor>()
                .ToDictionary(i => i.Id, i => i.PersonalInformation.Name);
        }

        var quizzes = await mongoContext.Quizzes
            .Find(q => courseIds.Contains(q.CourseId))
            .ToListAsync(ct);

        var attempts = await mongoContext.QuizAttempts
            .Find(a => a.StudentId == request.StudentId)
            .ToListAsync(ct);

        var quizCourseMap = quizzes.ToDictionary(q => q.Id, q => q.CourseId);
        var attemptCountsByCourse = attempts
            .GroupBy(a => quizCourseMap.GetValueOrDefault(a.QuizId))
            .ToDictionary(g => g.Key, g => g.Count());

        var enrollments = enrollmentsList.Select(sc =>
        {
            var course = courseMap.GetValueOrDefault(sc.CourseId);
            return new EnrollmentDto(
                sc.Id,
                sc.CourseId,
                course?.Name ?? string.Empty,
                new EnrollmentInstructorDto(
                    course?.InstructorId ?? Guid.Empty,
                    course?.InstructorId is not null
                        ? instructorNames.GetValueOrDefault(course.InstructorId.Value, string.Empty)
                        : string.Empty),
                new EnrollmentStudentDto(
                    sc.StudentId,
                    studentName,
                    attemptCountsByCourse.GetValueOrDefault(sc.CourseId, 0)),
                sc.EnrolledOnUtc);
        }).ToList();

        return enrollments;
    }
}
