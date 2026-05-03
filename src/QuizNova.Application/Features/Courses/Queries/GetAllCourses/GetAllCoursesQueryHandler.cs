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

        var courses = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(course => new CourseDto(
                course.Id,
                course.Name,
                dbContext.Instructors
                    .Where(instructor => instructor.Id == course.InstructorId)
                    .Select(instructor => instructor.PersonalInformation.Name)
                    .FirstOrDefault() ?? string.Empty,
                dbContext.StudentCourses.Count(studentCourse => studentCourse.CourseId == course.Id),
                dbContext.Quizzes.Count(quiz => quiz.CourseId == course.Id)))
            .ToListAsync(ct);

        var response = new PaginatedList<CourseDto>(
            courses,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} courses for page {PageNumber}", courses.Count, request.PageNumber);

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
                dbContext.StudentCourses.Any(sc => sc.StudentId == request.StudentId.Value && sc.CourseId == course.Id));
        }

        if (request.EnrolledStudentsCount.HasValue)
        {
            query = query.Where(course =>
                dbContext.StudentCourses.Count(studentCourse => studentCourse.CourseId == course.Id) ==
                request.EnrolledStudentsCount.Value);
        }

        if (request.QuizzesCount.HasValue)
        {
            query = query.Where(course =>
                dbContext.Quizzes.Count(quiz => quiz.CourseId == course.Id) ==
                request.QuizzesCount.Value);
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
