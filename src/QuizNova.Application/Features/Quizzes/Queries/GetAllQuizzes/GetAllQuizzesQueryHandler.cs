using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed class GetAllQuizzesQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetAllQuizzesQueryHandler> logger)
    : IRequestHandler<GetAllQuizzesQuery, Result<PaginatedList<QuizDto>>>
{
    public async Task<Result<PaginatedList<QuizDto>>> Handle(GetAllQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all quizzes");

        var filterBuilder = Builders<Quiz>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter &= filterBuilder.Text(request.SearchTerm);
        }

        if (request.Marks.HasValue)
        {
            filter &= filterBuilder.Where(q => q.Questions.Sum(question => question.Marks) == request.Marks.Value);
        }

        var totalCount = (int)await mongoContext.Quizzes.CountDocumentsAsync(filter, cancellationToken: ct);

        var quizzes = await mongoContext.Quizzes
            .Find(filter)
            .SortByDescending(q => q.StartsAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(ct);

        var courseIds = quizzes.Select(q => q.CourseId).Distinct().ToList();
        var instructorIds = quizzes.Select(q => q.InstructorId).Distinct().ToList();

        var courses = await dbContext.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        var instructors = await dbContext.Instructors
            .Where(i => instructorIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.PersonalInformation.Name, ct);

        var now = DateTimeOffset.UtcNow;

        var quizDtos = quizzes.Select(quiz => new QuizDto
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            CourseName = courses.GetValueOrDefault(quiz.CourseId, string.Empty),
            InstructorName = instructors.GetValueOrDefault(quiz.InstructorId, string.Empty),
            Marks = quiz.Questions.Sum(question => question.Marks),
            StartsAtUtc = quiz.StartsAtUtc,
            EndsAtUtc = quiz.EndsAtUtc,
            ServerUtc = now,
            State = quiz.StartsAtUtc > now ? "Upcoming" : quiz.EndsAtUtc < now ? "Completed" : "Active",
            CourseId = quiz.CourseId,
            InstructorId = quiz.InstructorId,
        }).ToList();

        var response = new PaginatedList<QuizDto>(
            quizDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} quizzes for page {PageNumber}", quizDtos.Count,
            request.PageNumber);

        return response;
    }
}

