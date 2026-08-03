using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;

public sealed class GetInstructorQuizzesQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetInstructorQuizzesQueryHandler> logger)
    : IRequestHandler<GetInstructorQuizzesQuery, Result<List<QuizDto>>>
{
    public async Task<Result<List<QuizDto>>> Handle(GetInstructorQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quizzes for instructor with ID: {InstructorId}", request.InstructorId);

        var instructor = await mongoContext.Users
            .Find(u => u.Id == request.InstructorId)
            .FirstOrDefaultAsync(ct) as Instructor;

        if (instructor is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var quizzes = await mongoContext.Quizzes
            .Find(q => q.InstructorId == request.InstructorId)
            .SortBy(q => q.StartsAtUtc)
            .ToListAsync(ct);

        logger.LogInformation("Successfully retrieved {Count} quizzes for instructor {InstructorId}",
            quizzes.Count, request.InstructorId);

        return quizzes.Select(q => q.ToQuizDto()).ToList();
    }
}
