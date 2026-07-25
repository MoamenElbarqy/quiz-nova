using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;

public sealed class UpdateQuizMetadataCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<UpdateQuizMetadataCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateQuizMetadataCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateQuizMetadataCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating quiz metadata for quiz: {QuizId}", request.QuizId);

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == request.QuizId)
            .FirstOrDefaultAsync(ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        var updateResult = quiz.Update(
            request.Title,
            request.StartsAtUtc,
            request.EndsAtUtc);

        if (updateResult.IsError)
        {
            logger.LogWarning(
                "Failed to update quiz {QuizId}: {Error}",
                request.QuizId,
                updateResult.TopError.Description);
            return updateResult.TopError;
        }

        await mongoContext.Quizzes.ReplaceOneAsync(q => q.Id == quiz.Id, quiz, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Quizzes], ct);

        logger.LogInformation("Successfully updated quiz metadata for quiz: {QuizId}", request.QuizId);

        return Result.Updated;
    }
}
