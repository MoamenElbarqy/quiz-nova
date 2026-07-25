using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsById;

public sealed record GetStudentEnrollmentsByIdQuery(Guid StudentId)
    : ICachedQuery<Result<List<EnrollmentDto>>>
{
    public string CacheKey => $"enrollments:student:{StudentId}";

    public string[] Tags => [CacheTags.Enrollments, CacheTags.Students];

}
