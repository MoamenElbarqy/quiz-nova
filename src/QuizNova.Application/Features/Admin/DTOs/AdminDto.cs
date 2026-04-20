namespace QuizNova.Application.Features.Admin.DTOs;

public sealed record AdminDto(
    Guid AdminId,
    string Name,
    string Email,
    string Password,
    string PhoneNumber);
