using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Application.Features.Admins.DTOs;

public sealed record AdminDto(
    Guid Id,
    PersonalInformationDto PersonalInformation);
