using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAllInstructorsQueryHandler> logger)
    : IRequestHandler<GetAllInstructorsQuery, Result<PaginatedList<InstructorDto>>>
{
    public async Task<Result<PaginatedList<InstructorDto>>> Handle(GetAllInstructorsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all instructors");

        var filter = Builders<User>.Filter.Where(u => u is Instructor);
        filter = ApplySearchTerm(filter, request);

        var allInstructorIds = (await mongoContext.Users
            .Find(filter)
            .Project(u => u.Id)
            .ToListAsync(ct)).ToHashSet();

        var courseCounts = await mongoContext.Courses
            .Aggregate()
            .Match(c => c.InstructorId != null && allInstructorIds.Contains(c.InstructorId!.Value))
            .Group(c => c.InstructorId!.Value, g => new { InstructorId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var courseCountDict = courseCounts.ToDictionary(x => x.InstructorId, x => x.Count);

        var quizCounts = await mongoContext.Quizzes
            .Aggregate()
            .Match(q => allInstructorIds.Contains(q.InstructorId))
            .Group(q => q.InstructorId, g => new { InstructorId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var quizCountDict = quizCounts.ToDictionary(x => x.InstructorId, x => x.Count);

        var filteredIds = ApplyCountFilters(allInstructorIds, courseCountDict, quizCountDict, request);

        filter &= Builders<User>.Filter.In(u => u.Id, filteredIds);

        var totalCount = filteredIds.Count;

        var instructors = await mongoContext.Users
            .Find(filter)
            .SortBy(u => u.PersonalInformation.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(ct);

        var instructorDtos = instructors
            .Cast<Instructor>()
            .Select(instructor => instructor.ToInstructorDto(
                courseCountDict.GetValueOrDefault(instructor.Id, 0),
                quizCountDict.GetValueOrDefault(instructor.Id, 0)))
            .ToList();

        var response = new PaginatedList<InstructorDto>(
            instructorDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation(
            "Successfully retrieved {Count} instructors for page {PageNumber}",
            instructorDtos.Count,
            request.PageNumber);

        return response;
    }

    private static HashSet<Guid> ApplyCountFilters(
        HashSet<Guid> instructorIds,
        Dictionary<Guid, int> courseCountDict,
        Dictionary<Guid, int> quizCountDict,
        GetAllInstructorsQuery request)
    {
        if (request.CoursesCount.HasValue)
        {
            instructorIds = instructorIds
                .Where(id => courseCountDict.GetValueOrDefault(id, 0) == request.CoursesCount.Value)
                .ToHashSet();
        }

        if (request.QuizzesCount.HasValue)
        {
            instructorIds = instructorIds
                .Where(id => quizCountDict.GetValueOrDefault(id, 0) == request.QuizzesCount.Value)
                .ToHashSet();
        }

        return instructorIds;
    }

    private static FilterDefinition<User> ApplySearchTerm(
        FilterDefinition<User> filter,
        GetAllInstructorsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return filter;
        }

        return filter & Builders<User>.Filter.Where(instructor =>
            instructor.PersonalInformation.Name.Contains(request.SearchTerm) ||
            instructor.PersonalInformation.Email.Contains(request.SearchTerm));
    }
}
