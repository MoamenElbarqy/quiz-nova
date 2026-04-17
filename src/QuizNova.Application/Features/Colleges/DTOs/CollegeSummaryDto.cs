namespace QuizNova.Application.Features.Colleges.DTOs;

public sealed record CollegeSummaryDto(
    int TotalStudents,
    int TotalInstructors,
    int TotalCourses);
