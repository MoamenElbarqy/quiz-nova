using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Mappers;

public static class ReactMapper
{
    public static ReactDto ToDto(this Reaction reaction)
    {
        return new ReactDto(
            reaction.Id,
            reaction.MessageId,
            reaction.ReactorId,
            reaction.Emoji,
            reaction.CreatedAt);
    }
}
