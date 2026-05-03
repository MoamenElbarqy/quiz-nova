using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery(
    string? SearchTerm = null,
    int? EnrolledStudentsCount = null,
    int? QuizzesCount = null,
    Guid? InstructorId = null,
    Guid? StudentId = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<CourseDto>>>
{
    public string CacheKey => $"courses:all:{SearchTerm}:{EnrolledStudentsCount}:{QuizzesCount}:{InstructorId}:{StudentId}:{PageNumber}:{PageSize}";

    public string[] Tags => ["courses"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
