using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Application.Features.CourseChats.DTOs;

public sealed record CourseChatRoomDto(
    Guid Id,
    Guid CourseId,
    Guid? InstructorId,
    ChatStatus Status,
    List<StudentDto> Students,
    List<MessageDto> Messages);
