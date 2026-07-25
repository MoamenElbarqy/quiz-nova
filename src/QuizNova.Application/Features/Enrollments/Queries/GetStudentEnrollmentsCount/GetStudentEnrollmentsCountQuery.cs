using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;

public sealed record GetStudentEnrollmentsCountQuery(Guid StudentId)
    : ICachedQuery<Result<EnrollmentCountDto>>
{
    public string CacheKey => $"enrollments:student:{StudentId}:count";

    public string[] Tags => [CacheTags.Enrollments, CacheTags.Students];

}
