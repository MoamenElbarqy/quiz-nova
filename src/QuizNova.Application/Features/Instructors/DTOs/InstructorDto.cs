namespace QuizNova.Application.Features.Instructors.DTOs;

public sealed record InstructorDto(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    int CoursesCount,
    int QuizzesCount);
