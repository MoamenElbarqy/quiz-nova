namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record EnrollmentDto(
    Guid CourseId,
    string CourseName,
    string InstructorName,
    int QuizzesCount,
    DateTimeOffset EnrolledOnUtc);
