using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;

public sealed class GetInstructorQuizzesQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetInstructorQuizzesQueryHandler> logger)
    : IRequestHandler<GetInstructorQuizzesQuery, Result<List<QuizDto>>>
{
    public async Task<Result<List<QuizDto>>> Handle(GetInstructorQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quizzes for instructor with ID: {InstructorId}", request.InstructorId);

        var instructorExists = await dbContext.Instructors
            .AsNoTracking()
            .AnyAsync(instructor => instructor.Id == request.InstructorId, ct);

        if (!instructorExists)
        {
            logger.LogWarning("Retrieval failed: Instructor with ID {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.InstructorNotFound(request.InstructorId);
        }

        var quizzes = await dbContext.Quizzes
            .AsNoTracking()
            .Where(quiz => quiz.InstructorId == request.InstructorId)
            .Include(quiz => quiz.Instructor)
            .Include(quiz => quiz.Course)
            .Include(quiz => quiz.Questions)
            .ThenInclude((Question question) => (question as Mcq)!.Choices)
            .AsSplitQuery()
            .OrderBy(quiz => quiz.StartsAtUtc)
            .ToListAsync(ct);

        var mappedQuizzes = quizzes
            .Select(quiz => quiz.ToQuizDto(
                quiz.Course!.Name,
                quiz.Instructor!.PersonalInformation.Name))
            .ToList();

        logger.LogInformation("Successfully retrieved {Count} quizzes for instructor {InstructorId}",
            mappedQuizzes.Count, request.InstructorId);

        return mappedQuizzes;
    }
}
