using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

public sealed class GetQuizByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetQuizByIdQueryHandler> logger)
    : IRequestHandler<GetQuizByIdQuery, Result<QuizDetailsDto>>
{
    public async Task<Result<QuizDetailsDto>> Handle(GetQuizByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz details for ID: {QuizId}", request.QuizId);

        var quiz = await dbContext.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .Include(q => q.Questions.OfType<Mcq>())
            .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, ct);

        if (quiz is null)
        {
            logger.LogWarning("Retrieval failed: Quiz with ID {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        logger.LogInformation("Successfully retrieved details for quiz {QuizId}", request.QuizId);

        return quiz.ToQuizDetailsDto();
    }
}
