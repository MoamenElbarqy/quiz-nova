using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

public sealed class GetStudentQuizAttemptsCountQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentQuizAttemptsCountQueryHandler> logger)
    : IRequestHandler<GetStudentQuizAttemptsCountQuery, Result<QuizAttemptsCountDto>>
{
    public async Task<Result<QuizAttemptsCountDto>> Handle(GetStudentQuizAttemptsCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz attempts count for student with ID: {StudentId}", request.StudentId);

        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.StudentNotFound(request.StudentId);
        }

        var quizAttemptCount = await dbContext.QuizAttempts
            .AsNoTracking()
            .CountAsync(quizAttempt => quizAttempt.StudentId == request.StudentId, ct);

        logger.LogInformation("Successfully retrieved quiz attempts count for student {StudentId}: {Count}", request.StudentId, quizAttemptCount);

        return new QuizAttemptsCountDto(quizAttemptCount);
    }
}

