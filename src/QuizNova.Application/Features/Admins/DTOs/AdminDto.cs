namespace QuizNova.Application.Features.Admins.DTOs;

public sealed record AdminDto(
    Guid AdminId,
    string Name,
    string Email,
    string Password,
    string PhoneNumber);
