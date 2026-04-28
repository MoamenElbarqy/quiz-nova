using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Enums;

namespace QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;

public sealed class GetStudentQuizzesQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentQuizzesQueryHandler> logger)
    : IRequestHandler<GetStudentQuizzesQuery, Result<StudentQuizzesDto>>
{
    public async Task<Result<StudentQuizzesDto>> Handle(GetStudentQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving available quizzes for student with ID: {StudentId}", request.StudentId);

        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var serverUtc = DateTimeOffset.UtcNow;

        var enrolledCourseIds = await dbContext.StudentCourses
            .AsNoTracking()
            .Where(studentCourse => studentCourse.StudentId == request.StudentId)
            .Select(studentCourse => studentCourse.CourseId)
            .ToListAsync(ct);

        if (enrolledCourseIds.Count == 0)
        {
            logger.LogInformation("No enrolled courses found for student {StudentId}", request.StudentId);
            return new StudentQuizzesDto(serverUtc, Array.Empty<StudentQuizDto>());
        }

        var quizzes = await dbContext.Quizzes
            .AsNoTracking()
            .Where(quiz => enrolledCourseIds.Contains(quiz.CourseId) && quiz.EndsAtUtc >= serverUtc)
            .Include(quiz => quiz.Course)
            .Include(quiz => quiz.Questions)
            .OrderBy(quiz => quiz.StartsAtUtc)
            .ToListAsync(ct);

        var mappedQuizzes = quizzes
            .Select(quiz => quiz.ToStudentQuizDto())
            .Where(quiz =>
                quiz.QuizStatus == QuizStatus.AvailableNow ||
                quiz.QuizStatus == QuizStatus.Scheduled)
            .ToList();

        logger.LogInformation("Successfully retrieved {Count} available/scheduled quizzes for student {StudentId}", mappedQuizzes.Count, request.StudentId);

        return new StudentQuizzesDto(serverUtc, mappedQuizzes);
    }
}
