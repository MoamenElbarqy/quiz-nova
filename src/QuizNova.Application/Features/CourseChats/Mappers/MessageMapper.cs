using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.Mappers;

public static class MessageMapper
{
    public static MessageDto ToDto(this Message message, UserDto sender, List<ReactDto> reacts)
    {
        return new MessageDto(
            message.Id,
            message.RoomId,
            sender,
            message.ReplyOnId,
            message.CreatedAt,
            message.Content,
            reacts);
    }
}
