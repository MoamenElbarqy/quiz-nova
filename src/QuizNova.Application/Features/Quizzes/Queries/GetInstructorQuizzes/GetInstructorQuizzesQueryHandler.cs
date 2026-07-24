using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;

public sealed class GetInstructorQuizzesQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetInstructorQuizzesQueryHandler> logger)
    : IRequestHandler<GetInstructorQuizzesQuery, Result<List<QuizDto>>>
{
    public async Task<Result<List<QuizDto>>> Handle(GetInstructorQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quizzes for instructor with ID: {InstructorId}", request.InstructorId);

        var instructor = await dbContext.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.InstructorId, ct);

        if (instructor is null)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var quizzes = await mongoContext.Quizzes
            .Find(q => q.InstructorId == request.InstructorId)
            .SortBy(q => q.StartsAtUtc)
            .ToListAsync(ct);

        var courseIds = quizzes.Select(q => q.CourseId).Distinct().ToList();
        var courses = await dbContext.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        var mappedQuizzes = quizzes
            .Select(quiz => quiz.ToQuizDto(
                courses.GetValueOrDefault(quiz.CourseId, string.Empty),
                instructor.PersonalInformation.Name))
            .ToList();

        logger.LogInformation("Successfully retrieved {Count} quizzes for instructor {InstructorId}",
            mappedQuizzes.Count, request.InstructorId);

        return mappedQuizzes;
    }
}

