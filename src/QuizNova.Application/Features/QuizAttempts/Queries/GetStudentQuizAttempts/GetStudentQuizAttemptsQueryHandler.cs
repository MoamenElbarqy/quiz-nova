using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

public sealed class GetStudentQuizAttemptsQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetStudentQuizAttemptsQueryHandler> logger)
    : IRequestHandler<GetStudentQuizAttemptsQuery, Result<List<QuizAttemptDto>>>
{
    public async Task<Result<List<QuizAttemptDto>>> Handle(GetStudentQuizAttemptsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz attempts for student with ID: {StudentId}", request.StudentId);

        var studentExists = await mongoContext.Users
            .Find(u => u.Id == request.StudentId && u is Student)
            .AnyAsync(ct);

        if (!studentExists)
        {
            logger.LogWarning("Retrieval failed: Student with ID {StudentId} not found", request.StudentId);
            return ApplicationErrors.QuizAttemptStudentNotFound(request.StudentId);
        }

        var attempts = await mongoContext.QuizAttempts
            .Find(quizAttempt => quizAttempt.StudentId == request.StudentId)
            .ToListAsync(ct);

        var quizIds = attempts.Select(a => a.QuizId).Distinct().ToList();
        var quizzes = await mongoContext.Quizzes
            .Find(q => quizIds.Contains(q.Id))
            .ToListAsync(ct);

        var quizMap = quizzes.ToDictionary(q => q.Id);

        var response = attempts
            .Select(attempt => attempt.ToQuizAttemptDto(quizMap.GetValueOrDefault(attempt.QuizId)))
            .ToList();

        logger.LogInformation("Successfully retrieved {Count} quiz attempts for student {StudentId}", response.Count, request.StudentId);

        return response;
    }
}
