namespace QuizNova.Api.DTOs.Requests;

public sealed record UpdateInstructorRequest(
    string Name,
    string Email,
    string Password,
    string PhoneNumber);
