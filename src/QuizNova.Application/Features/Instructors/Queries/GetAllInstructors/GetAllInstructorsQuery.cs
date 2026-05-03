using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;

public sealed record GetAllInstructorsQuery(
    int PageNumber,
    int PageSize,
    string? SearchTerm,
    int? CoursesCount,
    int? QuizzesCount)
    : ICachedQuery<Result<PaginatedList<InstructorDto>>>
{
    public string CacheKey => $"instructors:all:{PageNumber}:{PageSize}:{SearchTerm}:{CoursesCount}:{QuizzesCount}";

    public string[] Tags => ["instructors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
