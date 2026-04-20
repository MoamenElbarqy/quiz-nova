namespace QuizNova.Api.DTOs.Requests;

public sealed record UpdateAdminRequest(
    string Name,
    string Email,
    string Password,
    string PhoneNumber);
