namespace QuizNova.Application.Features.Colleges.DTOs;

public sealed record CollegeSummaryDto(
    Guid CollegeId,
    string CollegeName,
    int TotalStudents,
    int TotalInstructors,
    int TotalDepartments,
    int TotalCourses);
