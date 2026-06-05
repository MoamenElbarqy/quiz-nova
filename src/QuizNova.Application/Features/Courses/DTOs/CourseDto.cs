namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record CourseDto(
    Guid Id,
    string CourseName,
    Guid? InstructorId,
    string? InstructorName,
    int EnrolledStudentsCount,
    int QuizzesCount,
    int RemainingMarks);
