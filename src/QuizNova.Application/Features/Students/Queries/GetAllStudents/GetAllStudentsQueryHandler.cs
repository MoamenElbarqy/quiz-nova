using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAllStudentsQueryHandler> logger)
    : IRequestHandler<GetAllStudentsQuery, Result<PaginatedList<StudentDto>>>
{
    public async Task<Result<PaginatedList<StudentDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all students");

        var studentFilter = Builders<User>.Filter.Where(u => u is Student);
        studentFilter = ApplySearchTerm(studentFilter, request);
        studentFilter = await ApplyFilteringAsync(studentFilter, request, ct);

        var sortedUsers = mongoContext.Users
            .Find(studentFilter)
            .SortBy(u => u.PersonalInformation.Name);

        var totalCount = (int)await mongoContext.Users.CountDocumentsAsync(studentFilter, cancellationToken: ct);

        var students = await sortedUsers
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(ct);

        var studentList = students.Cast<Student>().ToList();
        var studentIds = studentList.Select(s => s.Id).ToList();

        var enrollmentCounts = studentIds.Count != 0
            ? (await mongoContext.Enrollments
                .Aggregate()
                .Match(e => studentIds.Contains(e.StudentId))
                .Group(e => e.StudentId, g => new { StudentId = g.Key, Count = g.Count() })
                .ToListAsync(ct))
                .ToDictionary(x => x.StudentId, x => x.Count)
            : new Dictionary<Guid, int>();

        var studentDtos = studentList
            .Select(student => student.ToStudentDto(enrollmentCounts.GetValueOrDefault(student.Id, 0)))
            .ToList();

        var response = new PaginatedList<StudentDto>(
            studentDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} students for page {PageNumber}", studentDtos.Count, request.PageNumber);

        return response;
    }

    private static FilterDefinition<User> ApplySearchTerm(
        FilterDefinition<User> filter,
        GetAllStudentsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return filter;
        }

        return filter & Builders<User>.Filter.Where(student =>
            student.PersonalInformation.Name.Contains(request.SearchTerm) ||
            student.PersonalInformation.Email.Contains(request.SearchTerm));
    }

    private async Task<FilterDefinition<User>> ApplyFilteringAsync(
        FilterDefinition<User> filter,
        GetAllStudentsQuery request,
        CancellationToken ct)
    {
        if (request.EnrolledCoursesCount.HasValue)
        {
            var enrollmentCounts = await mongoContext.Enrollments
                .Aggregate()
                .Group(e => e.StudentId, g => new { StudentId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var matchingIds = enrollmentCounts
                .Where(x => x.Count == request.EnrolledCoursesCount.Value)
                .Select(x => x.StudentId)
                .ToHashSet();

            if (request.EnrolledCoursesCount.Value == 0)
            {
                var allEnrolledIds = enrollmentCounts.Select(x => x.StudentId).ToHashSet();
                if (allEnrolledIds.Count > 0)
                {
                    filter &= Builders<User>.Filter.Nin(u => u.Id, allEnrolledIds);
                }
            }
            else
            {
                filter &= Builders<User>.Filter.In(u => u.Id, matchingIds);
            }
        }

        if (request.CourseId.HasValue && request.IsEnrolledInCourse.HasValue)
        {
            var enrolledStudentIds = await mongoContext.Enrollments
                .Find(e => e.CourseId == request.CourseId.Value)
                .Project(e => e.StudentId)
                .ToListAsync(ct);

            var enrolledSet = enrolledStudentIds.ToHashSet();

            if (request.IsEnrolledInCourse.Value)
            {
                filter &= Builders<User>.Filter.In(u => u.Id, enrolledSet);
            }
            else
            {
                filter &= Builders<User>.Filter.Nin(u => u.Id, enrolledSet);
            }
        }

        return filter;
    }
}
