using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public sealed class GetAllQuizzesAttemptsQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAllQuizzesAttemptsQueryHandler> logger)
    : IRequestHandler<GetAllQuizzesAttemptsQuery, Result<PaginatedList<QuizAttemptDto>>>
{
    public async Task<Result<PaginatedList<QuizAttemptDto>>> Handle(
        GetAllQuizzesAttemptsQuery request,
        CancellationToken ct)
    {
        logger.LogInformation("Retrieving all quiz attempts");

        var filterBuilder = Builders<QuizAttempt>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();

            var matchingStudentIds = await mongoContext.Users
                .Find(u => u is Student && u.PersonalInformation.Name.Contains(searchTerm))
                .Project(u => u.Id)
                .ToListAsync(ct);

            var matchingQuizIds = await mongoContext.Quizzes
                .Find(Builders<Quiz>.Filter.Text(searchTerm))
                .Project(q => q.Id)
                .ToListAsync(ct);

            filter &= filterBuilder.In(a => a.StudentId, matchingStudentIds) |
                      filterBuilder.In(a => a.QuizId, matchingQuizIds);
        }

        var allMatchingAttempts = await mongoContext.QuizAttempts
            .Find(filter)
            .SortByDescending(quizAttempt => quizAttempt.SubmittedAt)
            .ToListAsync(ct);

        if (request.CorrectAnswers.HasValue)
        {
            allMatchingAttempts = allMatchingAttempts
                .Where(a => a.StudentAnswers.OfType<AutoGradedAnswer>().Count(ans => ans.IsCorrect) == request.CorrectAnswers.Value)
                .ToList();
        }

        var totalCount = allMatchingAttempts.Count;

        var pagedAttempts = allMatchingAttempts
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var quizIds = pagedAttempts.Select(a => a.QuizId).Distinct().ToList();
        var quizzes = await mongoContext.Quizzes
            .Find(q => quizIds.Contains(q.Id))
            .ToListAsync(ct);

        var quizMap = quizzes.ToDictionary(q => q.Id);

        var response = pagedAttempts
            .Select(attempt => attempt.ToQuizAttemptDto(quizMap.GetValueOrDefault(attempt.QuizId)))
            .ToList();

        var paginatedResponse = new PaginatedList<QuizAttemptDto>(
            response,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} quiz attempts for page {PageNumber}", response.Count,
            request.PageNumber);

        return paginatedResponse;
    }
}
