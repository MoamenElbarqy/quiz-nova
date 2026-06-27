using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Application.Features.Quizzes.Commands.DeleteQuestion;

public sealed class DeleteQuestionCommandHandler(
    IAppDbContext dbContext,
    ILogger<DeleteQuestionCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<DeleteQuestionCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteQuestionCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Deleting question {QuestionId} from quiz {QuizId}",
            request.QuestionId,
            request.QuizId);

        var quiz = await dbContext.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        var question = quiz.Questions.FirstOrDefault(q => q.Id == request.QuestionId);

        if (question is null)
        {
            logger.LogWarning("Question {QuestionId} not found in quiz {QuizId}", request.QuestionId, request.QuizId);
            return QuizErrors.QuestionNotFound;
        }

        var deleteResult = quiz.DeleteQuestion(question);

        if (deleteResult.IsError)
        {
            logger.LogWarning(
                "Failed to delete question {QuestionId}: {Error}",
                request.QuestionId,
                deleteResult.TopError.Description);
            return deleteResult.TopError;
        }

        dbContext.Questions.Remove(question);
        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["quizzes"], ct);

        logger.LogInformation(
            "Successfully deleted question {QuestionId} from quiz {QuizId}",
            request.QuestionId,
            request.QuizId);

        return Result.Deleted;
    }
}
