namespace QuizNova.Api.DTOs.Requests;

public sealed record CreateStudentRequest(
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role);
