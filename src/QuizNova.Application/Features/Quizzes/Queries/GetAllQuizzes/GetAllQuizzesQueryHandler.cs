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
    ILogger<GetAllQuizzesQueryHandler> logger)
    : IRequestHandler<GetAllQuizzesQuery, Result<PaginatedList<QuizDto>>>
{
    public async Task<Result<PaginatedList<QuizDto>>> Handle(GetAllQuizzesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all quizzes");

        IQueryable<Quiz> query = dbContext.Quizzes
            .AsNoTracking()
            .AsQueryable();

        query = ApplySearchTerm(query, request, dbContext);
        query = ApplyFiltering(query, request);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);
        var now = DateTimeOffset.UtcNow;

        var quizzes = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(quiz => new QuizDto(
                quiz.Id,
                quiz.Title,
                dbContext.Courses.Where(course => course.Id == quiz.CourseId)
                .Select(course => course.Name)
                .FirstOrDefault() ?? string.Empty,
                dbContext.Instructors
                .Where(instructor => instructor.Id == quiz.InstructorId)
                .Select(instructor => instructor.PersonalInformation.Name)
                .FirstOrDefault() ?? string.Empty,
                quiz.Questions.Sum(question => question.Marks),
                quiz.StartsAtUtc,
                quiz.EndsAtUtc,
                now,
                quiz.StartsAtUtc > now ? "Upcoming" : quiz.EndsAtUtc < now ? "Completed" : "Active"))
            .ToListAsync(ct);

        var response = new PaginatedList<QuizDto>(
            quizzes,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} quizzes for page {PageNumber}", quizzes.Count, request.PageNumber);

        return response;
    }

    private static IQueryable<Quiz> ApplyFiltering(
        IQueryable<Quiz> query,
        GetAllQuizzesQuery request)
    {
        if (request.Marks.HasValue)
        {
            query = query.Where(quiz => quiz.Questions.Sum(question => question.Marks) == request.Marks.Value);
        }

        return query;
    }

    private static IQueryable<Quiz> ApplySearchTerm(
        IQueryable<Quiz> query,
        GetAllQuizzesQuery request,
        IAppDbContext dbContext)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        return query.Where(quiz =>
            quiz.Title.Contains(request.SearchTerm) ||
            dbContext.Courses
                .Where(course => course.Id == quiz.CourseId)
                .Select(course => course.Name)
                .FirstOrDefault()!
                .Contains(request.SearchTerm) ||
            dbContext.Instructors
                .Where(instructor => instructor.Id == quiz.InstructorId)
                .Select(instructor => instructor.PersonalInformation.Name)
                .FirstOrDefault()!
                .Contains(request.SearchTerm));
    }

    private static IOrderedQueryable<Quiz> ApplySorting(IQueryable<Quiz> query)
    {
        return query.OrderByDescending(quiz => quiz.StartsAtUtc);
    }
}
