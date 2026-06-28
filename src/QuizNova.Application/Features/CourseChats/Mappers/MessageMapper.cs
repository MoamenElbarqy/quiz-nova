using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Mappers;

public static class MessageMapper
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto(
            message.Id,
            message.RoomId,
            message.SenderId,
            message.ReplyOnId,
            message.CreatedAt,
            message.Content);
    }
}
