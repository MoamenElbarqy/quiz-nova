using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;

public sealed record GetInstructorCoursesQuery(Guid InstructorId)
    : ICachedQuery<Result<List<CourseDto>>>
{
    public string CacheKey => $"courses:instructor:{InstructorId}";

    public string[] Tags => ["courses", "instructors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
