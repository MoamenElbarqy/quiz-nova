using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizCourseId;

public sealed class UpdateQuizCourseIdCommandHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<UpdateQuizCourseIdCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateQuizCourseIdCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateQuizCourseIdCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Updating course ID for quiz {QuizId} to {NewCourseId}",
            request.QuizId,
            request.NewCourseId);

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == request.QuizId)
            .FirstOrDefaultAsync(ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        var courseExists = await dbContext.Courses
            .AnyAsync(c => c.Id == request.NewCourseId, ct);

        if (!courseExists)
        {
            logger.LogWarning("Course {CourseId} not found", request.NewCourseId);
            return ApplicationErrors.QuizCourseNotFound(request.NewCourseId);
        }

        var updateResult = quiz.UpdateCourseId(request.NewCourseId);

        if (updateResult.IsError)
        {
            logger.LogWarning(
                "Failed to update course ID for quiz {QuizId}: {Error}",
                request.QuizId,
                updateResult.TopError.Description);
            return updateResult.TopError;
        }

        await mongoContext.Quizzes.ReplaceOneAsync(q => q.Id == quiz.Id, quiz, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync(["quizzes", "courses"], ct);

        logger.LogInformation(
            "Successfully updated course ID for quiz {QuizId} to {NewCourseId}",
            request.QuizId,
            request.NewCourseId);

        return Result.Updated;
    }
}
