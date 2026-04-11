namespace QuizNova.Api.DTOs.Requests;

public sealed record LoginRequest(
    string email,
    string password);
