namespace QuizNova.Application.Features.Courses.DTOs;

public sealed record CoursePerformanceDto(
    Guid Id,
    string Name,
    string InstructorName,
    int NumberOfStudents,
    double AvgScore);
