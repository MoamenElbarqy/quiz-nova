using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetAllInstructorsQueryHandler> logger)
    : IRequestHandler<GetAllInstructorsQuery, Result<PaginatedList<InstructorDto>>>
{
    public async Task<Result<PaginatedList<InstructorDto>>> Handle(GetAllInstructorsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all instructors");

        var query = dbContext.Instructors
            .AsNoTracking()
            .AsQueryable();

        query = ApplySearchTerm(query, request);
        query = ApplyFiltering(query, request, dbContext);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);

        var instructors = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(instructor => new InstructorDto(
                instructor.Id,
                instructor.PersonalInformation.Name,
                instructor.PersonalInformation.Email,
                instructor.PersonalInformation.Password,
                instructor.PersonalInformation.PhoneNumber,
                dbContext.Courses.Count(course => course.InstructorId == instructor.Id),
                dbContext.Quizzes.Count(quiz => quiz.InstructorId == instructor.Id)))
            .ToListAsync(ct);

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

    private static IQueryable<Domain.Entities.Users.Instructors.Instructor> ApplyFiltering(
        IQueryable<Domain.Entities.Users.Instructors.Instructor> query,
        GetAllInstructorsQuery request,
        IAppDbContext dbContext)
    {
        if (request.CoursesCount.HasValue)
        {
            query = query.Where(instructor =>
                dbContext.Courses.Count(course => course.InstructorId == instructor.Id) ==
                request.CoursesCount.Value);
        }

        if (request.QuizzesCount.HasValue)
        {
            query = query.Where(instructor =>
                dbContext.Quizzes.Count(quiz => quiz.InstructorId == instructor.Id) ==
                request.QuizzesCount.Value);
        }

        return query;
    }

    private static IQueryable<Domain.Entities.Users.Instructors.Instructor> ApplySearchTerm(
        IQueryable<Domain.Entities.Users.Instructors.Instructor> query,
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

    private static IOrderedQueryable<Domain.Entities.Users.Instructors.Instructor> ApplySorting(IQueryable<Domain.Entities.Users.Instructors.Instructor> query)
    {
        return query.OrderBy(instructor => instructor.PersonalInformation.Name);
    }
}
