using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

public sealed class GetStudentQuizAttemptsQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetStudentQuizAttemptsQueryHandler> logger)
    : IRequestHandler<GetStudentQuizAttemptsQuery, Result<List<QuizAttemptDto>>>
{
    public async Task<Result<List<QuizAttemptDto>>> Handle(GetStudentQuizAttemptsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz attempts for student with ID: {StudentId}", request.StudentId);

        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.QuizAttemptStudentNotFound(request.StudentId);
        }

        var attempts = await dbContext.QuizAttempts
            .AsNoTracking()
            .Where(quizAttempt => quizAttempt.StudentId == request.StudentId)
            .Include(quizAttempt => quizAttempt.Quiz)
            .ThenInclude(quiz => quiz!.Questions)
            .Include(quizAttempt => quizAttempt.StudentAnswers)
            .ToListAsync(ct);

        var response = attempts
            .Select(attempt => attempt.ToQuizAttemptDto())
            .ToList();

        logger.LogInformation("Successfully retrieved {Count} quiz attempts for student {StudentId}", response.Count, request.StudentId);

        return response;
    }
}
