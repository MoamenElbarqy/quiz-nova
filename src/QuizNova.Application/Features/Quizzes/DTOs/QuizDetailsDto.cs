namespace QuizNova.Application.Features.Quizzes.DTOs;

public sealed class QuizDetailsDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid CourseId { get; init; }
    public Guid InstructorId { get; init; }
    public DateTimeOffset StartsAtUtc { get; init; }
    public DateTimeOffset EndsAtUtc { get; init; }
    public IReadOnlyCollection<QuestionDto> Questions { get; init; } = Array.Empty<QuestionDto>();
}
