using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.MongoDb;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;

public sealed class GetQuizAttemptByIdQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetQuizAttemptByIdQueryHandler> logger)
    : IRequestHandler<GetQuizAttemptByIdQuery, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(
        GetQuizAttemptByIdQuery request,
        CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz attempt with ID: {QuizAttemptId}", request.QuizAttemptId);

        var quizAttempt = await mongoContext.QuizAttempts
            .GetAttemptWithQuizAsync(qa => qa.Id == request.QuizAttemptId, ct);

        if (quizAttempt is null)
        {
            logger.LogWarning("Retrieval failed: Quiz attempt with ID {QuizAttemptId} not found",
                request.QuizAttemptId);
            return ApplicationErrors.QuizAttemptNotFound(request.QuizAttemptId);
        }

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == quizAttempt.QuizId)
            .FirstOrDefaultAsync(ct);

        logger.LogInformation("Successfully retrieved quiz attempt {QuizAttemptId}", request.QuizAttemptId);

        return quizAttempt.ToQuizAttemptDto(quiz);
    }
}

