using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetAllStudentsQueryHandler> logger)
    : IRequestHandler<GetAllStudentsQuery, Result<PaginatedList<StudentDto>>>
{
    public async Task<Result<PaginatedList<StudentDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all students");

        IQueryable<Student> query = dbContext.Students
            .AsNoTracking()
            .AsQueryable();

        query = ApplySearchTerm(query, request);
        query = ApplyFiltering(query, request, dbContext);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);

        var students = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(student => new StudentDto(
                student.Id,
                student.PersonalInformation.Name,
                student.PersonalInformation.Email,
                student.PersonalInformation.Password,
                student.PersonalInformation.PhoneNumber,
                dbContext.StudentCourses.Count(studentCourse => studentCourse.StudentId == student.Id)))
            .ToListAsync(ct);

        var response = new PaginatedList<StudentDto>(
            students,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} students for page {PageNumber}", students.Count, request.PageNumber);

        return response;
    }

    private static IQueryable<Student> ApplyFiltering(
        IQueryable<Student> query,
        GetAllStudentsQuery request,
        IAppDbContext dbContext)
    {
        if (request.EnrolledCoursesCount.HasValue)
        {
            query = query.Where(student =>
                dbContext.StudentCourses.Count(studentCourse => studentCourse.StudentId == student.Id) ==
                request.EnrolledCoursesCount.Value);
        }

        return query;
    }

    private static IQueryable<Student> ApplySearchTerm(
        IQueryable<Student> query,
        GetAllStudentsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        return query.Where(student =>
            student.PersonalInformation.Name.Contains(request.SearchTerm) ||
            student.PersonalInformation.Email.Contains(request.SearchTerm));
    }

    private static IOrderedQueryable<Student> ApplySorting(IQueryable<Student> query)
    {
        return query.OrderBy(student => student.PersonalInformation.Name);
    }
}
