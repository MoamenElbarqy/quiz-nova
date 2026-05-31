namespace QuizNova.Application.Features.Admins.DTOs;

public sealed record AdminDto(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber);
