using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestionManually;

public sealed class GradeQuestionManuallyCommandHandler(
    IAppDbContext dbContext,
    ILogger<GradeQuestionManuallyCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<GradeQuestionManuallyCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(GradeQuestionManuallyCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Grading answer {AnswerId} with score {Score}",
            request.AnswerId,
            request.Score);

        var answer = await dbContext.ManuallyGradedAnswers
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.Id == request.AnswerId, ct);

        if (answer is null)
        {
            logger.LogWarning(
                "Manual grading failed: Answer {AnswerId} not found",
                request.AnswerId);

            return ApplicationErrors.AnswerNotFound(request.AnswerId);
        }

        var gradeResult = answer.Grade(request.Score);

        if (gradeResult.IsError)
        {
            logger.LogWarning(
                "Manual grading failed for answer {AnswerId}: {ErrorDescription}",
                request.AnswerId,
                gradeResult.TopError.Description);

            return gradeResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["quiz-attempts"], ct);

        logger.LogInformation(
            "Successfully graded answer {AnswerId} with score {Score}",
            request.AnswerId,
            request.Score);

        return Result.Updated;
    }
}
