using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Enums;

namespace QuizNova.Application.Features.QuizAttempts.Commands.StartQuizAttempt;

public sealed class StartQuizAttemptCommandHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    IUser user,
    ILogger<StartQuizAttemptCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<StartQuizAttemptCommand, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(StartQuizAttemptCommand request, CancellationToken ct)
    {
        var studentId = Guid.Parse(user.Id!);

        logger.LogInformation(
            "Starting quiz attempt for student {StudentId} and quiz {QuizId}",
            studentId,
            request.QuizId);

        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(s => s.Id == studentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Start attempt failed: Student {StudentId} not found", studentId);
            return ApplicationErrors.QuizAttemptStudentNotFound(studentId);
        }

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == request.QuizId)
            .FirstOrDefaultAsync(ct);

        if (quiz is null)
        {
            logger.LogWarning("Start attempt failed: Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        var isStudentEnrolled = await dbContext.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == quiz.CourseId, ct);

        if (!isStudentEnrolled)
        {
            logger.LogWarning(
                "Start attempt failed: Student {StudentId} not enrolled in course {CourseId}",
                studentId,
                quiz.CourseId);

            return ApplicationErrors.StudentNotEnrolledInCourse(studentId, quiz.CourseId);
        }

        var existingActiveAttempt = await mongoContext.QuizAttempts
            .Find(qa => qa.StudentId == studentId && qa.QuizId == request.QuizId
                                                  && qa.Status == QuizAttemptStatus.InProgress)
            .AnyAsync(ct);

        if (existingActiveAttempt)
        {
            logger.LogWarning(
                "Start attempt failed: Active attempt already exists for student {StudentId} and quiz {QuizId}",
                studentId,
                request.QuizId);

            return ApplicationErrors.QuizAttemptAlreadyExists(studentId, request.QuizId);
        }

        var existingCompletedAttempt = await mongoContext.QuizAttempts
            .Find(qa => qa.StudentId == studentId && qa.QuizId == request.QuizId
                                                  && qa.Status == QuizAttemptStatus.Completed)
            .AnyAsync(ct);

        if (existingCompletedAttempt)
        {
            logger.LogWarning(
                "Start attempt failed: Completed attempt already exists for student {StudentId} and quiz {QuizId}",
                studentId,
                request.QuizId);

            return ApplicationErrors.QuizAttemptAlreadyCompleted(studentId, request.QuizId);
        }

        var createResult = quiz.StartAttempt(studentId);

        if (createResult.IsError)
        {
            logger.LogWarning(
                "Start attempt failed: Domain error. {Error}",
                createResult.TopError.Description);

            return createResult.TopError;
        }

        var attempt = createResult.Value;

        await mongoContext.QuizAttempts.InsertOneAsync(attempt, cancellationToken: ct);

        await cacheInvalidator.InvalidateAsync(["quiz_attempts", "quizzes"], ct);

        logger.LogInformation(
            "Successfully started quiz attempt {QuizAttemptId} for student {StudentId}",
            attempt.Id,
            studentId);

        return attempt.ToQuizAttemptDto();
    }
}
