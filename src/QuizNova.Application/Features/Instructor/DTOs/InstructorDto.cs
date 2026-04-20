namespace QuizNova.Application.Features.Instructor.DTOs;

public sealed record InstructorDto(
    Guid InstructorId,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    int CourseCount,
    int QuizCount);
