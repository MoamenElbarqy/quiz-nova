namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateCourseRequest(
    Guid Id,
    string Name,
    Guid? InstructorId,
    int MinimumPassingMarks,
    int MaximumMarks);
