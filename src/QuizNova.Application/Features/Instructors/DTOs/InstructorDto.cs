using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Application.Features.Instructors.DTOs;

public sealed record InstructorDto(
    Guid Id,
    PersonalInformationDto PersonalInformation,
    int CoursesCount,
    int QuizzesCount);
