namespace QuizNova.Application.Features.Instructor.DTOs;

public sealed record InstructorDto(
    Guid InstructorId,
    string Name,
    IReadOnlyList<string> Departments,
    int CourseCount,
    int StudentCount);
