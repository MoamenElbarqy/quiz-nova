using QuizNova.Application.Features.Students.DTOs;
namespace QuizNova.Application.Features.CourseChats.DTOs;

public sealed record CourseChatRoomDto(
    Guid Id,
    Guid CourseId,
    Guid? InstructorId,
    List<StudentDto> Students,
    List<MessageDto> Messages);
