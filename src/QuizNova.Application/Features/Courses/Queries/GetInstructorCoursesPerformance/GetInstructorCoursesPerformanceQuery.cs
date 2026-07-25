using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;

public sealed record GetInstructorCoursesPerformanceQuery(Guid InstructorId)
    : ICachedQuery<Result<List<CoursePerformanceDto>>>
{
    public string CacheKey => $"courses:instructor:{InstructorId}:performance";

    public string[] Tags => [CacheTags.Courses, CacheTags.Quizzes, CacheTags.Instructors, CacheTags.Performance];

}
