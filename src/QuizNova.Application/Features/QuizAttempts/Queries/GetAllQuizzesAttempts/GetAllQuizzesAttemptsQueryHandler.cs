using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public sealed class GetAllQuizzesAttemptsQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetAllQuizzesAttemptsQueryHandler> logger)
    : IRequestHandler<GetAllQuizzesAttemptsQuery, Result<PaginatedList<QuizAttemptDto>>>
{
    public async Task<Result<PaginatedList<QuizAttemptDto>>> Handle(GetAllQuizzesAttemptsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all quiz attempts");

        IQueryable<QuizAttempt> query = dbContext.QuizAttempts
            .AsNoTracking()
            .AsQueryable();

        query = ApplySearchTerm(query, request, dbContext);
        query = ApplyFiltering(query, request);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);

        var attempts = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(quizAttempt => quizAttempt.Quiz)
            .ThenInclude(quiz => quiz!.Questions)
            .Include(quizAttempt => quizAttempt.StudentAnswers)
            .ToListAsync(ct);

        var response = attempts
            .Select(attempt => attempt.ToQuizAttemptDto())
            .ToList();

        var paginatedResponse = new PaginatedList<QuizAttemptDto>(
            response,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} quiz attempts for page {PageNumber}", response.Count, request.PageNumber);

        return paginatedResponse;
    }

    private static IQueryable<QuizAttempt> ApplyFiltering(
        IQueryable<QuizAttempt> query,
        GetAllQuizzesAttemptsQuery request)
    {
        if (request.CorrectAnswers.HasValue)
        {
            query = query.Where(quizAttempt =>
                quizAttempt.StudentAnswers.Count(answer => answer.IsCorrect) ==
                request.CorrectAnswers.Value);
        }

        return query;
    }

    private static IQueryable<QuizAttempt> ApplySearchTerm(
        IQueryable<QuizAttempt> query,
        GetAllQuizzesAttemptsQuery request,
        IAppDbContext dbContext)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        return query.Where(quizAttempt =>
            dbContext.Quizzes
                .Where(quiz => quiz.Id == quizAttempt.QuizId)
                .Select(quiz => quiz.Title)
                .FirstOrDefault()!
                .Contains(request.SearchTerm) ||
            dbContext.Students
                .Where(student => student.Id == quizAttempt.StudentId)
                .Select(student => student.PersonalInformation.Name)
                .FirstOrDefault()!
                .Contains(request.SearchTerm));
    }

    private static IOrderedQueryable<QuizAttempt> ApplySorting(IQueryable<QuizAttempt> query)
    {
        return query.OrderByDescending(quizAttempt => quizAttempt.SubmittedAt);
    }
}
