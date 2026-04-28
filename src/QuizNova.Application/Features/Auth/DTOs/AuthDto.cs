namespace QuizNova.Application.Features.Auth.DTOs;

public sealed record AuthDto(
    TokenDto Token,
    UserDto User);
