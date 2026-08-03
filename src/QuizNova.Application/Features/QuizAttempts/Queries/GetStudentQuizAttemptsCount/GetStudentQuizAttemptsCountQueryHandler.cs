using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

public sealed class GetStudentQuizAttemptsCountQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetStudentQuizAttemptsCountQueryHandler> logger)
    : IRequestHandler<GetStudentQuizAttemptsCountQuery, Result<QuizAttemptsCountDto>>
{
    public async Task<Result<QuizAttemptsCountDto>> Handle(GetStudentQuizAttemptsCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz attempts count for student with ID: {StudentId}", request.StudentId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == request.StudentId && u is Student)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var quizAttemptCount = (int)await mongoContext.QuizAttempts
            .CountDocumentsAsync(quizAttempt => quizAttempt.StudentId == request.StudentId, cancellationToken: ct);

        logger.LogInformation("Successfully retrieved quiz attempts count for student {StudentId}: {Count}", request.StudentId, quizAttemptCount);

        return new QuizAttemptsCountDto(quizAttemptCount);
    }
}
