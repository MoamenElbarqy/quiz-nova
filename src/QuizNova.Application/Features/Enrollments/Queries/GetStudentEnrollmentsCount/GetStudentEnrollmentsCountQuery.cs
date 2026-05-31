using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;

public sealed record GetStudentEnrollmentsCountQuery(Guid StudentId)
    : ICachedQuery<Result<EnrollmentCountDto>>
{
    public string CacheKey => $"enrollments:student:{StudentId}:count";

    public string[] Tags => ["enrollments", "students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
