using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed record GetAllStudentsQuery(
    string? SearchTerm = null,
    int? EnrolledCoursesCount = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<StudentDto>>>
{
    public string CacheKey => $"students:all:{PageNumber}:{PageSize}:{SearchTerm}:{EnrolledCoursesCount}";

    public string[] Tags => ["students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
