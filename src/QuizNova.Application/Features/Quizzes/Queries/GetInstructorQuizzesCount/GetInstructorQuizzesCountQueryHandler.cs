using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;

public sealed class GetInstructorQuizzesCountQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetInstructorQuizzesCountQueryHandler> logger)
    : IRequestHandler<GetInstructorQuizzesCountQuery, Result<QuizzesCountDto>>
{
    public async Task<Result<QuizzesCountDto>> Handle(GetInstructorQuizzesCountQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quizzes count for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId, ct);
        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var quizzesCount = await dbContext.Quizzes.CountAsync(quiz => quiz.InstructorId == request.InstructorId, ct);

        logger.LogInformation("Successfully retrieved quizzes count for instructor {InstructorId}: {Count}", request.InstructorId, quizzesCount);

        return new QuizzesCountDto(quizzesCount);
    }
}

