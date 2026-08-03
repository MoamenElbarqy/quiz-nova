using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.MongoDb;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.CompleteQuizAttempt;

public sealed class CompleteQuizAttemptCommandHandler(
    IMongoDbContext mongoContext,
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

        var attempt = await mongoContext.QuizAttempts
            .GetAttemptWithQuizAsync(a => a.Id == request.AttemptId, ct);

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
            request.SubmittedAt.UtcDateTime);

        if (completeResult.IsError)
        {
            logger.LogWarning(
                "Complete attempt failed: Domain error. {Error}",
                completeResult.TopError.Description);

            return completeResult.TopError;
        }

        await mongoContext.QuizAttempts.ReplaceOneAsync(a => a.Id == attempt.Id, attempt, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.QuizAttempts, CacheTags.Quizzes], ct);

        logger.LogInformation(
            "Successfully completed quiz attempt {AttemptId}. Score: {Score}",
            request.AttemptId,
            attempt.Score);

        return attempt.ToQuizAttemptDto();
    }
}
