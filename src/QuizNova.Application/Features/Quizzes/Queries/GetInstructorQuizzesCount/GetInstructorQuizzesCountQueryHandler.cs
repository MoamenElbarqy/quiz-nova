using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;

public sealed class GetInstructorQuizzesCountQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetInstructorQuizzesCountQueryHandler> logger)
    : IRequestHandler<GetInstructorQuizzesCountQuery, Result<QuizzesCountDto>>
{
    public async Task<Result<QuizzesCountDto>> Handle(GetInstructorQuizzesCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quizzes count for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await mongoContext.Users
            .Find(u => u.Id == request.InstructorId && u is Instructor)
            .AnyAsync(ct);

        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var quizzesCount =
            (int)await mongoContext.Quizzes.CountDocumentsAsync(quiz => quiz.InstructorId == request.InstructorId,
                cancellationToken: ct);

        logger.LogInformation("Successfully retrieved quizzes count for instructor {InstructorId}: {Count}",
            request.InstructorId, quizzesCount);

        return new QuizzesCountDto(quizzesCount);
    }
}
