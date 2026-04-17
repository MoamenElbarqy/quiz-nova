namespace QuizNova.Application.Features.Students.DTOs;

public sealed record StudentDto(
    Guid StudentId,
    string Name,
    int EnrolledCourseCount);
