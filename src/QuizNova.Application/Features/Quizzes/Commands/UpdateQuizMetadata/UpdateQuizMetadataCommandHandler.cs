using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;

public sealed class UpdateQuizMetadataCommandHandler(
    IAppDbContext dbContext,
    ILogger<UpdateQuizMetadataCommandHandler> logger)
    : IRequestHandler<UpdateQuizMetadataCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateQuizMetadataCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating quiz metadata for quiz: {QuizId}", request.QuizId);

        var quiz = await dbContext.Quizzes
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, ct);

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

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully updated quiz metadata for quiz: {QuizId}", request.QuizId);

        return Result.Updated;
    }
}
