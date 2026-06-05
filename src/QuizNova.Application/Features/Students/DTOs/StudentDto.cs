using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Application.Features.Students.DTOs;

public sealed record StudentDto(
    Guid Id,
    PersonalInformationDto PersonalInformation,
    int EnrolledCoursesCount);
