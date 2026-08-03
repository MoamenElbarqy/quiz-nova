using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAllCoursesQueryHandler> logger)
    : IRequestHandler<GetAllCoursesQuery, Result<PaginatedList<CourseDto>>>
{
    public async Task<Result<PaginatedList<CourseDto>>> Handle(GetAllCoursesQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all courses");

        var filterBuilder = Builders<Course>.Filter;
        var filter = filterBuilder.Empty;
        filter = await ApplyFilteringAsync(filter, request, ct);
        filter = ApplySearchTerm(filter, request);

        var totalCount = (int)await mongoContext.Courses.CountDocumentsAsync(filter, cancellationToken: ct);

        var coursesList = await mongoContext.Courses
            .Find(filter)
            .SortBy(c => c.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(ct);

        var courseIds = coursesList.Select(c => c.Id).ToList();
        var instructorIds = coursesList
            .Select(c => c.InstructorId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

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
                course.EnrollmentsCount,
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

    private static FilterDefinition<Course> ApplySearchTerm(
        FilterDefinition<Course> filter,
        GetAllCoursesQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return filter;
        }

        return filter & Builders<Course>.Filter.Where(course =>
            course.Name.Contains(request.SearchTerm));
    }

    private async Task<FilterDefinition<Course>> ApplyFilteringAsync(
        FilterDefinition<Course> filter,
        GetAllCoursesQuery request,
        CancellationToken ct)
    {
        if (request.InstructorId.HasValue)
        {
            filter &= Builders<Course>.Filter.Eq(course => course.InstructorId, request.InstructorId.Value);
        }

        if (request.StudentId.HasValue)
        {
            var enrolledCourseIds = await mongoContext.Enrollments
                .Find(e => e.StudentId == request.StudentId.Value)
                .Project(e => e.CourseId)
                .ToListAsync(ct);

            filter &= Builders<Course>.Filter.In(course => course.Id, enrolledCourseIds);
        }

        if (request.EnrolledStudentsCount.HasValue)
        {
            var courseEnrollmentCounts = await mongoContext.Enrollments
                .Aggregate()
                .Group(e => e.CourseId, g => new { CourseId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var matchingCourseIds = courseEnrollmentCounts
                .Where(x => x.Count == request.EnrolledStudentsCount.Value)
                .Select(x => x.CourseId)
                .ToList();

            if (request.EnrolledStudentsCount.Value == 0)
            {
                var allEnrolledCourseIds = courseEnrollmentCounts.Select(x => x.CourseId).ToHashSet();
                if (allEnrolledCourseIds.Count > 0)
                {
                    filter &= Builders<Course>.Filter.Nin(course => course.Id, allEnrolledCourseIds);
                }
            }
            else
            {
                filter &= Builders<Course>.Filter.In(course => course.Id, matchingCourseIds);
            }
        }

        if (request.QuizzesCount.HasValue)
        {
            var courseQuizCounts = await mongoContext.Quizzes
                .Aggregate()
                .Group(q => q.CourseId, g => new { CourseId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var matchingCourseIds = courseQuizCounts
                .Where(x => x.Count == request.QuizzesCount.Value)
                .Select(x => x.CourseId)
                .ToList();

            if (request.QuizzesCount.Value == 0)
            {
                var allCoursesWithQuizzes = courseQuizCounts.Select(x => x.CourseId).ToHashSet();
                if (allCoursesWithQuizzes.Count > 0)
                {
                    filter &= Builders<Course>.Filter.Nin(course => course.Id, allCoursesWithQuizzes);
                }
            }
            else
            {
                filter &= Builders<Course>.Filter.In(course => course.Id, matchingCourseIds);
            }
        }

        return filter;
    }
}
