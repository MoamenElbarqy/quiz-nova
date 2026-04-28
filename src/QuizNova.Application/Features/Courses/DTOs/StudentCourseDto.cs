namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record StudentCourseDto(
    Guid CourseId,
    string CourseName,
    string InstructorName,
    int QuizzesCount,
    DateTimeOffset EnrolledOnUtc);
