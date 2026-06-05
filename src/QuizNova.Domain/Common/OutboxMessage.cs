namespace QuizNova.Domain.Common;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Content { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}
