using System.Text.Json;

namespace QuizNova.Application.Features.CourseChats.DTOs;

public sealed record MessageDto(
    Guid Id,
    Guid RoomId,
    Guid SenderId,
    Guid? ReplyOnId,
    DateTimeOffset CreatedAt,
    JsonDocument Content);
