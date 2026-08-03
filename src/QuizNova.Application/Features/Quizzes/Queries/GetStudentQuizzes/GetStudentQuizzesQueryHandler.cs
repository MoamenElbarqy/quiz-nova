using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Enums;
using QuizNova.Domain.Entities.Quizzes.Enums;

namespace QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;

public sealed class GetStudentQuizzesQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetStudentQuizzesQueryHandler> logger)
    : IRequestHandler<GetStudentQuizzesQuery, Result<StudentQuizzesDto>>
{
    public async Task<Result<StudentQuizzesDto>> Handle(GetStudentQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving available quizzes for student with ID: {StudentId}", request.StudentId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == request.StudentId)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var serverUtc = DateTimeOffset.UtcNow;

        var enrolledCourseIds = await mongoContext.Enrollments
            .Find(enrollment => enrollment.StudentId == request.StudentId)
            .Project(enrollment => enrollment.CourseId)
            .ToListAsync(ct);

        var quizzes = await mongoContext.Quizzes
            .Find(quiz => enrolledCourseIds.Contains(quiz.CourseId) && quiz.EndsAtUtc >= serverUtc)
            .SortBy(quiz => quiz.StartsAtUtc)
            .ToListAsync(ct);

        var quizIds = quizzes.Select(q => q.Id).ToList();

        var studentAttempts = await mongoContext.QuizAttempts
            .Find(a => a.StudentId == request.StudentId && quizIds.Contains(a.QuizId))
            .Project(a => new { a.QuizId, a.Id, a.Status })
            .ToListAsync(ct);

        var attemptByQuizId = studentAttempts
            .GroupBy(a => a.QuizId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(a => a.Status == QuizAttemptStatus.Completed).First());

        var mappedQuizzes = quizzes
            .Select(quiz =>
            {
                var attempt = attemptByQuizId.GetValueOrDefault(quiz.Id);
                return quiz.ToStudentQuizDto(
                    attempt?.Id,
                    attempt?.Status.ToString());
            })
            .Where(quiz => quiz.QuizStatus is QuizStatus.AvailableNow or QuizStatus.Scheduled)
            .ToList();

        logger.LogInformation("Successfully retrieved {Count} available/scheduled quizzes for student {StudentId}",
            mappedQuizzes.Count, request.StudentId);

        return new StudentQuizzesDto(serverUtc, mappedQuizzes);
    }
}
