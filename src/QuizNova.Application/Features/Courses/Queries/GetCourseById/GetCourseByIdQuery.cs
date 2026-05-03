using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery(Guid CourseId) : ICachedQuery<Result<CourseDto>>
{
    public string CacheKey => $"courses:{CourseId}";

    public string[] Tags => ["courses"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
