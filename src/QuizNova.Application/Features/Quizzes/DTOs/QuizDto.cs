namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed class QuizDto
{
    public Guid QuizId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public string InstructorName { get; init; } = string.Empty;
    public int Marks { get; init; }
    public DateTimeOffset StartsAtUtc { get; init; }
    public DateTimeOffset EndsAtUtc { get; init; }
    public DateTimeOffset ServerUtc { get; init; }
    public string State { get; init; } = string.Empty;
    public Guid CourseId { get; init; }
    public Guid InstructorId { get; init; }
    public IReadOnlyCollection<QuestionDto> Questions { get; init; } = Array.Empty<QuestionDto>();
}
