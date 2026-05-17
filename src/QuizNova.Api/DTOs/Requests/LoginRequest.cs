namespace QuizNova.Api.DTOs.Requests;

public sealed record LoginRequest(
    string Email,
    string Password);
