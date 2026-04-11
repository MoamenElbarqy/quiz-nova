namespace QuizNova.Application.Features.Departments.DTOs;

public sealed record DepartmentDto(
    Guid DepartmentId,
    string DepartmentName,
    int StudentCount,
    int InstructorCount,
    int CourseCount);
