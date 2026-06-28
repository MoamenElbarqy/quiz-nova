using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Mappers;

public static class ReactMapper
{
    public static ReactDto ToDto(this React react)
    {
        return new ReactDto(
            react.Id,
            react.MessageId,
            react.ReactorId,
            react.Emoji,
            react.CreatedAt);
    }
}
