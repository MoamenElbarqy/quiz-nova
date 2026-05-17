namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed class QuizDto
{
    public Guid QuizId { get; init; }
    public required string Title { get; init; }
    public required string CourseName { get; init; }
    public required string InstructorName { get; init; }
    public int Marks { get; init; }
    public DateTimeOffset StartsAtUtc { get; init; }
    public DateTimeOffset EndsAtUtc { get; init; }
    public DateTimeOffset ServerUtc { get; init; }
    public required string State { get; init; }
    public Guid CourseId { get; init; }
    public Guid InstructorId { get; init; }
    public IReadOnlyCollection<QuestionDto> Questions { get; init; } = Array.Empty<QuestionDto>();
}
