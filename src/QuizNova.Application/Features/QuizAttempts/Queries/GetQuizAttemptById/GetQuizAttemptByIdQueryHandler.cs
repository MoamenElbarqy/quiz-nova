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

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;

public sealed class GetQuizAttemptByIdQueryHandler(
    IAppDbContext context,
    ILogger<GetQuizAttemptByIdQueryHandler> logger)
    : IRequestHandler<GetQuizAttemptByIdQuery, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(
        GetQuizAttemptByIdQuery request,
        CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz attempt with ID: {QuizAttemptId}", request.QuizAttemptId);

        var quizAttempt = await context.QuizAttempts
            .Include(qa => qa.Quiz)
                .ThenInclude(q => q!.Questions)
                    .ThenInclude((Question question) => (question as Mcq)!.Choices)
            .Include(qa => qa.StudentAnswers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(qa => qa.Id == request.QuizAttemptId, ct);

        if (quizAttempt is null)
        {
            logger.LogWarning("Retrieval failed: Quiz attempt with ID {QuizAttemptId} not found", request.QuizAttemptId);
            return ApplicationErrors.QuizAttemptNotFound(request.QuizAttemptId);
        }

        logger.LogInformation("Successfully retrieved quiz attempt {QuizAttemptId}", request.QuizAttemptId);

        return quizAttempt.ToQuizAttemptDto();
    }
}
