namespace QuizNova.Api.DTOs.Requests;

public sealed record UpdateStudentRequest(
    string Name,
    string Email,
    string Password,
    string PhoneNumber);
