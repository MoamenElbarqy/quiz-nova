using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;

public sealed record GetInstructorCoursesCountQuery(Guid InstructorId)
    : ICachedQuery<Result<CoursesCountDto>>
{
    public string CacheKey => $"courses:instructor:{InstructorId}:count";

    public string[] Tags => [CacheTags.Courses, CacheTags.Instructors];

}
