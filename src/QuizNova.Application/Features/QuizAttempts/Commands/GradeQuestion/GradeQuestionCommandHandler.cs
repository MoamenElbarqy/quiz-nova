using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

namespace QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestion;

public sealed class GradeQuestionCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<GradeQuestionCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<GradeQuestionCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(GradeQuestionCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Grading answer {AnswerId} with score {Score}",
            request.AnswerId,
            request.Score);

        var filter = Builders<QuizAttempt>.Filter.ElemMatch(
            a => a.StudentAnswers,
            Builders<QuestionAnswer>.Filter.Eq(ans => ans.Id, request.AnswerId));

        var attempt = await mongoContext.QuizAttempts
            .Find(filter)
            .FirstOrDefaultAsync(ct);

        if (attempt is null)
        {
            logger.LogWarning(
                "Manual grading failed: Answer {AnswerId} not found",
                request.AnswerId);

            return ApplicationErrors.AnswerNotFound(request.AnswerId);
        }

        var answer = attempt.StudentAnswers.OfType<ManuallyGradedAnswers>()
            .FirstOrDefault(a => a.Id == request.AnswerId);
        if (answer is null)
        {
            return ApplicationErrors.AnswerNotFound(request.AnswerId);
        }

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == attempt.QuizId)
            .FirstOrDefaultAsync(ct);

        if (quiz is not null)
        {
            attempt.AttachQuizQuestions(quiz.Questions);
        }

        var gradeResult = answer.Grade(request.Score, request.Feedback);

        if (gradeResult.IsError)
        {
            logger.LogWarning(
                "Manual grading failed for answer {AnswerId}: {ErrorDescription}",
                request.AnswerId,
                gradeResult.TopError.Description);

            return gradeResult.TopError;
        }

        await mongoContext.QuizAttempts.ReplaceOneAsync(a => a.Id == attempt.Id, attempt, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.QuizAttempts], ct);

        logger.LogInformation(
            "Successfully graded answer {AnswerId} with score {Score}",
            request.AnswerId,
            request.Score);

        return Result.Updated;
    }
}

