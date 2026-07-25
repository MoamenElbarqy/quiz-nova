using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Enrollments.Queries.GetAllCoursesEnrollmentCount;

public sealed record GetAllCoursesEnrollmentCountQuery()
    : ICachedQuery<Result<List<CourseEnrollmentCountDto>>>
{
    public string CacheKey => "courses:enrollment-counts";

    public string[] Tags => [CacheTags.Courses, CacheTags.Enrollments];

}
