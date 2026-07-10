using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Application.Features.QuizAttempts.Commands.CompleteQuizAttempt;

public sealed class CompleteQuizAttemptCommandHandler(
    IAppDbContext dbContext,
    IUser user,
    ILogger<CompleteQuizAttemptCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<CompleteQuizAttemptCommand, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(CompleteQuizAttemptCommand request, CancellationToken ct)
    {
        var studentId = Guid.Parse(user.Id!);

        logger.LogInformation(
            "Completing quiz attempt {AttemptId} for student {StudentId}",
            request.AttemptId,
            studentId);

        var attempt = await dbContext.QuizAttempts
            .Include(a => a.Quiz)
                .ThenInclude(q => q!.Questions)
                    .ThenInclude((Question question) => (question as Mcq)!.Choices)
            .Include(a => a.StudentAnswers)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, ct);

        if (attempt is null)
        {
            logger.LogWarning("Complete attempt failed: Attempt {AttemptId} not found", request.AttemptId);
            return ApplicationErrors.QuizAttemptNotFound(request.AttemptId);
        }

        if (attempt.StudentId != studentId)
        {
            return Error.Forbidden("Forbidden", "You do not own this attempt.");
        }

        var completeResult = attempt.Complete(
            request.SubmittedAt.UtcDateTime,
            attempt.Quiz!.EndsAtUtc.UtcDateTime);

        if (completeResult.IsError)
        {
            logger.LogWarning(
                "Complete attempt failed: Domain error. {Error}",
                completeResult.TopError.Description);

            return completeResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["quiz_attempts", "quizzes"], ct);

        logger.LogInformation(
            "Successfully completed quiz attempt {AttemptId}. Score: {Score}",
            request.AttemptId,
            attempt.Score);

        return attempt.ToQuizAttemptDto();
    }
}
