using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;

public sealed record GetInstructorCoursesPerformanceQuery(Guid InstructorId)
    : ICachedQuery<Result<List<CoursePerformanceDto>>>
{
    public string CacheKey => $"courses:instructor:{InstructorId}:performance";

    public string[] Tags => ["courses", "quizzes", "instructors", "performance"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
