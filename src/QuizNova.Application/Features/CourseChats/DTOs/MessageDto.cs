using System.Text.Json;

using QuizNova.Application.Features.Auth.DTOs;

namespace QuizNova.Application.Features.CourseChats.DTOs;

public sealed record MessageDto(
    Guid Id,
    Guid RoomId,
    UserDto Sender,
    Guid? ReplyOnId,
    DateTimeOffset CreatedAt,
    JsonDocument Content,
    List<ReactDto> Reacts);
