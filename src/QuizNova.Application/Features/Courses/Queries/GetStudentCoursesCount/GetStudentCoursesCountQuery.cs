using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetEnrollmentsCount;

public sealed record GetEnrollmentsCountQuery(Guid StudentId)
    : ICachedQuery<Result<CoursesCountDto>>
{
    public string CacheKey => $"courses:student:{StudentId}:count";

    public string[] Tags => ["courses", "students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
