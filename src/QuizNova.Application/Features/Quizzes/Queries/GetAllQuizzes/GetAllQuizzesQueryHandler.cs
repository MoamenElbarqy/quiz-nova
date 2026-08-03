using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed class GetAllQuizzesQueryHandler(
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

        var quizzes = await mongoContext.Quizzes
            .Find(filter)
            .SortByDescending(q => q.StartsAtUtc)
            .ToListAsync(ct);

        if (request.Marks.HasValue)
        {
            quizzes = [.. quizzes.Where(q => q.Questions.Sum(question => question.Marks) == request.Marks.Value)];
        }

        var totalCount = quizzes.Count;

        var pagedQuizzes = quizzes
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var courseIds = pagedQuizzes.Select(q => q.CourseId).Distinct().ToList();
        var instructorIds = pagedQuizzes.Select(q => q.InstructorId).Distinct().ToList();

        var courses = await mongoContext.Courses
            .Find(c => courseIds.Contains(c.Id))
            .ToListAsync(ct);
        var courseDict = courses.ToDictionary(c => c.Id, c => c.Name);

        var instructorNames = new Dictionary<Guid, string>();
        if (instructorIds.Count != 0)
        {
            var instructors = await mongoContext.Users
                .Find(u => instructorIds.Contains(u.Id) && u is Instructor)
                .ToListAsync(ct);
            instructorNames = instructors
                .Cast<Instructor>()
                .ToDictionary(i => i.Id, i => i.PersonalInformation.Name);
        }

        var now = DateTimeOffset.UtcNow;

        var quizDtos = pagedQuizzes.Select(quiz => new QuizDto
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            CourseName = courseDict.GetValueOrDefault(quiz.CourseId, string.Empty),
            InstructorName = instructorNames.GetValueOrDefault(quiz.InstructorId, string.Empty),
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
