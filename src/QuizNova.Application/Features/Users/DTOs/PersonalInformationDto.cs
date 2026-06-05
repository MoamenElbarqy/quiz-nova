namespace QuizNova.Application.Features.Users.DTOs;

public sealed record PersonalInformationDto(
    string Name,
    string Email,
    string PhoneNumber);
