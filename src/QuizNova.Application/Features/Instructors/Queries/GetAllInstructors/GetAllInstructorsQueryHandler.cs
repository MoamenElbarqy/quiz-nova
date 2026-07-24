using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetAllInstructorsQueryHandler> logger)
    : IRequestHandler<GetAllInstructorsQuery, Result<PaginatedList<InstructorDto>>>
{
    public async Task<Result<PaginatedList<InstructorDto>>> Handle(GetAllInstructorsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all instructors");

        var query = dbContext.Instructors
            .AsNoTracking()
            .Include(i => i.Courses)
            .AsQueryable();

        query = ApplySearchTerm(query, request);
        query = ApplyFiltering(query, request, dbContext);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);

        var instructorsList = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var instructorIds = instructorsList.Select(i => i.Id).ToList();

        var quizCounts = await mongoContext.Quizzes
            .Aggregate()
            .Match(q => instructorIds.Contains(q.InstructorId))
            .Group(q => q.InstructorId, g => new { InstructorId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var quizCountDict = quizCounts.ToDictionary(g => g.InstructorId, g => g.Count);

        var instructors = instructorsList
            .Select(instructor => instructor.ToInstructorDto(
                instructor.Courses.Count(),
                quizCountDict.GetValueOrDefault(instructor.Id, 0)))
            .ToList();

        var response = new PaginatedList<InstructorDto>(
            instructors,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation(
            "Successfully retrieved {Count} instructors for page {PageNumber}",
            instructors.Count,
            request.PageNumber);

        return response;
    }

    private static IQueryable<Instructor> ApplyFiltering(
        IQueryable<Instructor> query,
        GetAllInstructorsQuery request,
        IAppDbContext dbContext)
    {
        if (request.CoursesCount.HasValue)
        {
            query = query.Where(instructor =>
                dbContext.Courses.Count(course => course.InstructorId == instructor.Id) ==
                request.CoursesCount.Value);
        }

        return query;
    }

    private static IQueryable<Instructor> ApplySearchTerm(
        IQueryable<Instructor> query,
        GetAllInstructorsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        return query.Where(instructor =>
            instructor.PersonalInformation.Name.Contains(request.SearchTerm) ||
            instructor.PersonalInformation.Email.Contains(request.SearchTerm));
    }

    private static IOrderedQueryable<Instructor> ApplySorting(
        IQueryable<Instructor> query)
    {
        return query.OrderBy(instructor => instructor.PersonalInformation.Name);
    }
}
