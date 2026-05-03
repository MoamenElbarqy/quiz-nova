namespace QuizNova.Application.Features.Instructors.DTOs;

public sealed record InstructorDto(
    Guid InstructorId,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    int CoursesCount,
    int QuizzesCount);
