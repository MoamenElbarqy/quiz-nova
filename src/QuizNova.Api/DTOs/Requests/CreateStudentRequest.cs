namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateStudentRequest(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role);
