using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesById;

public sealed record GetStudentCoursesByIdQuery(Guid StudentId)
    : ICachedQuery<Result<List<StudentCourseDto>>>
{
    public string CacheKey => $"courses:student:{StudentId}";

    public string[] Tags => ["courses", "students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
