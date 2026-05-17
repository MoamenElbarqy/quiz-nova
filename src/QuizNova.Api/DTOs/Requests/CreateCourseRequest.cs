namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateCourseRequest(
    string Name,
    Guid? InstructorId,
    int MinimumPassingMarks,
    int MaximumMarks);
