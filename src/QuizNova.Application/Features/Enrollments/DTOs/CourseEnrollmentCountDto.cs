namespace QuizNova.Application.Features.Enrollments.DTOs;

public sealed record CourseEnrollmentCountDto(
    Guid CourseId,
    string CourseName,
    int EnrollmentsCount);
