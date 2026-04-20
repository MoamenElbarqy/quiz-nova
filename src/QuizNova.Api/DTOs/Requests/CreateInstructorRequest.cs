namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateInstructorRequest(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role);
