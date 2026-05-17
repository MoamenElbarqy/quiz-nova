namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateAdminRequest(
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role);
