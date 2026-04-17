namespace QuizNova.Application.Features.Instructor.DTOs;

public sealed record InstructorDto(
    Guid InstructorId,
    string Name,
    int CourseCount,
    int QuizCount);
