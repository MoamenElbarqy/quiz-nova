namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record CourseDto(
    Guid CourseId,
    string CourseName,
    IReadOnlyList<string> Departments,
    string InstructorName,
    int EnrolledStudentCount,
    int QuizCount);
