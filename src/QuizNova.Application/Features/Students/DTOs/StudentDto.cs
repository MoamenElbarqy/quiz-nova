namespace QuizNova.Application.Features.Students.DTOs;

public sealed record StudentDto(
    Guid StudentId,
    string Name,
    string DepartmentName,
    string LevelName,
    int EnrolledCourseCount);
