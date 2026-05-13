namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record CourseDto(
    Guid CourseId,
    string CourseName,
    Guid? InstructorId,
    string InstructorName,
    int EnrolledStudentsCount,
    int QuizzesCount,
    int RemainingMarks);
