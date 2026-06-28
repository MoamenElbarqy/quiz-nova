namespace QuizNova.Application.Features.CourseChats.DTOs;

public sealed record ReactDto(
    Guid Id,
    Guid MessageId,
    Guid ReactorId,
    string Emoji,
    DateTimeOffset CreatedAt);
