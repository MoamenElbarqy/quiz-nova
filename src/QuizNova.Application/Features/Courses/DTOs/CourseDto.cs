namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record CourseDto(
    Guid CourseId,
    string CourseName,
    string InstructorName,
    int EnrolledStudentsCount,
    int QuizzesCount);
