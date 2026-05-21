namespace QuizNova.Application.Features.Students.DTOs;

public sealed record StudentDto(
    Guid StudentId,
    string Name,
    string Email,
    string PhoneNumber,
    int EnrolledCoursesCount);
