using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetAllCoursesQueryHandler> logger)
    : IRequestHandler<GetAllCoursesQuery, Result<PaginatedList<CourseDto>>>
{
    public async Task<Result<PaginatedList<CourseDto>>> Handle(GetAllCoursesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all courses");

        IQueryable<Course> query = dbContext.Courses
            .AsNoTracking()
            .AsQueryable();

        query = ApplySearchTerm(query, request, dbContext);
        query = ApplyFiltering(query, request, dbContext);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);

        var coursesList = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var courseIds = coursesList.Select(c => c.Id).ToList();
        var instructorIds = coursesList.Select(c => c.InstructorId).Distinct().ToList();

        var instructorNames = await dbContext.Instructors
            .Where(i => instructorIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.PersonalInformation.Name, ct);

        var enrollmentsCounts = await dbContext.Enrollments
            .Where(e => courseIds.Contains(e.CourseId))
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CourseId, g => g.Count, ct);

        var quizzes = await mongoContext.Quizzes
            .Find(q => courseIds.Contains(q.CourseId))
            .ToListAsync(ct);

        var quizGroup = quizzes.GroupBy(q => q.CourseId).ToDictionary(g => g.Key, g => g.ToList());

        var courses = coursesList.Select(course =>
        {
            var courseQuizzes = quizGroup.TryGetValue(course.Id, out var qList) ? qList : [];
            var quizzesCount = courseQuizzes.Count;

            var consumedMarks = courseQuizzes.Sum(q => q.Questions.Sum(question => question.Marks));
            return new CourseDto(
                course.Id,
                course.Name,
                course.InstructorId,
                course.InstructorId.HasValue && instructorNames.TryGetValue(course.InstructorId.Value, out var iName)
                    ? iName
                    : string.Empty,
                enrollmentsCounts.GetValueOrDefault(course.Id, 0),
                quizzesCount,
                course.MaximumMarks - consumedMarks);
        }).ToList();

        var response = new PaginatedList<CourseDto>(
            courses,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} courses for page {PageNumber}", courses.Count,
            request.PageNumber);

        return response;
    }

    private static IQueryable<Course> ApplyFiltering(
        IQueryable<Course> query,
        GetAllCoursesQuery request,
        IAppDbContext dbContext)
    {
        if (request.InstructorId.HasValue)
        {
            query = query.Where(course => course.InstructorId == request.InstructorId.Value);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(course =>
                dbContext.Enrollments.Any(sc => sc.StudentId == request.StudentId.Value && sc.CourseId == course.Id));
        }

        if (request.EnrolledStudentsCount.HasValue)
        {
            query = query.Where(course =>
                dbContext.Enrollments.Count(enrollment => enrollment.CourseId == course.Id) ==
                request.EnrolledStudentsCount.Value);
        }

        return query;
    }

    private static IQueryable<Course> ApplySearchTerm(
        IQueryable<Course> query,
        GetAllCoursesQuery request,
        IAppDbContext dbContext)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        return query.Where(course =>
            course.Name.Contains(request.SearchTerm) ||
            dbContext.Instructors
                .Where(instructor => instructor.Id == course.InstructorId)
                .Select(instructor => instructor.PersonalInformation.Name)
                .FirstOrDefault()!
                .Contains(request.SearchTerm));
    }

    private static IOrderedQueryable<Course> ApplySorting(IQueryable<Course> query)
    {
        return query.OrderBy(course => course.Name);
    }
}
